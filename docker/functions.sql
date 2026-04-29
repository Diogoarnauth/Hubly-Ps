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
    recommendation_score INT  -- <--- Nova coluna com os pontos
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