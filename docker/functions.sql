CREATE OR REPLACE FUNCTION dbo.get_recommended_companies(
    p_user_id INT,
    p_interests JSONB
) 
RETURNS TABLE (
    user_id INT,
    company_name VARCHAR,
    description TEXT,
    company_size VARCHAR,
    website_link VARCHAR,
    country_headquarters VARCHAR,
    recommendation_score INT 
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        c.user_id,
        c.company_name,
        c.description,
        c.company_size,
        c.website_link,
        c.country_headquarters,
        (
            COALESCE(SUM((p_interests->'sectors'->>cs.sector_id::text)::int), 0) * 10 +
            COALESCE((p_interests->'countries'->>c.country_headquarters)::int, 0) * 5 +
            COALESCE((p_interests->'sizes'->>c.company_size)::int, 0) * 2
        )::int AS recommendation_score
    FROM dbo.companies c
    LEFT JOIN dbo.company_sectors cs ON c.user_id = cs.company_user_id
    WHERE c.user_id != p_user_id
      AND c.user_id NOT IN (
          SELECT viewed_company_id 
          FROM dbo.profile_views_history 
          WHERE viewer_user_id = p_user_id AND viewed_company_id IS NOT NULL
      )
    GROUP BY c.user_id, c.company_name, c.description, c.company_size, c.website_link, c.country_headquarters
    ORDER BY recommendation_score DESC
    LIMIT 15;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION dbo.get_recommended_social_profiles(
    p_user_id INT,
    p_interests JSONB 
) 
RETURNS TABLE (
    social_profile_id INT,
    recommendation_score INT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        csp.id,
        (
            COALESCE((
                SELECT SUM((p_interests->'sectors'->>cps.sector_id::text)::int)
                FROM dbo.creator_profile_sectors cps 
                WHERE cps.profile_id = csp.id
            ), 0) * 100 +

            -- 2. PLATAFORMA (Peso 10): relevante, mas secundário aos setores
            COALESCE((p_interests->'platforms'->>csp.platform_id::text)::int, 0) * 10 +

            -- 3. DISPONIBILIDADE DO CRIADOR (Peso 8)
            (CASE WHEN cr.availability_status = 'AVAILABLE' THEN 8 ELSE 0 END) +

            -- 4. RATING DO CRIADOR (Peso 5)
            (COALESCE(cr.global_rating, 0) * 5)::int +

            -- 5. PREÇO (Peso 5): influencia apenas se estiver dentro do range aceitável
            (CASE 
                WHEN p_interests ? 'avg_price'
                     AND (p_interests->>'avg_price')::decimal > 0
                     AND csp.price_min <= (p_interests->>'avg_price')::decimal * 1.2
                THEN 5 
                ELSE 0 
             END)
        )::int AS recommendation_score
    FROM dbo.creator_social_profiles csp
    JOIN dbo.creators cr ON csp.creator_id = cr.user_id
    WHERE cr.user_id != p_user_id
      AND csp.id NOT IN (
          SELECT viewed_social_profile_id 
          FROM dbo.profile_views_history 
          WHERE viewer_user_id = p_user_id AND viewed_social_profile_id IS NOT NULL
      )
    ORDER BY recommendation_score DESC
    LIMIT 15;
END;
$$ LANGUAGE plpgsql;