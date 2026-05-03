WITH params AS (
  SELECT $$ {"sectors":{"7":5,"1":2},"platforms":{"2":5,"1":2},"avg_price":178.57142857142858} $$::jsonb AS interests
)
SELECT
  csp.id,
  COALESCE((params.interests->'sectors'->>cps.sector_id::text)::int, 0) AS sector_freq,
  COALESCE((params.interests->'platforms'->>csp.platform_id::text)::int, 0) AS platform_freq,
  CASE WHEN cr.availability_status = 'AVAILABLE' THEN 8 ELSE 0 END AS avail_score,
  (COALESCE(cr.global_rating, 0) * 5)::int AS rating_score,
  CASE WHEN params.interests ? 'avg_price'
            AND (params.interests->>'avg_price')::decimal > 0
            AND csp.price_min <= (params.interests->>'avg_price')::decimal * 1.2
       THEN 5 ELSE 0 END AS price_score,
  COALESCE((
            SELECT SUM((params.interests->'sectors'->>cps2.sector_id::text)::int)
            FROM dbo.creator_profile_sectors cps2
            WHERE cps2.profile_id = csp.id
        ), 0) AS sector_sum,
  csp.platform_id,
  csp.price_min,
  cr.global_rating,
  csp.id IN (20,17,7) AS selected
FROM dbo.creator_social_profiles csp
JOIN dbo.creators cr ON csp.creator_id = cr.user_id
LEFT JOIN dbo.creator_profile_sectors cps ON cps.profile_id = csp.id
CROSS JOIN params
WHERE csp.id IN (20,17,7)
ORDER BY csp.id;
