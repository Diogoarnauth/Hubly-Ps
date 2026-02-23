
INSERT INTO dbo.social_platforms (name_platform) VALUES ('YouTube');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Instagram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Facebook');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('X');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Telegram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('TikTok');


INSERT INTO dbo.users (name, email, password_validation, created_at) 
VALUES ('João Criador', 'joao@hubly.com', 'hash_password_123', 1714560000000);

INSERT INTO dbo.users (name, email, password_validation, created_at) 
VALUES ('Empresa Global', 'contact@empresa.com', 'hash_password_456', 1714560000000);



INSERT INTO dbo.creators (user_id, artistic_name, content, audience) 
VALUES (1, 'JoaoVlogs', 'Tecnologia e Viagens', 'Jovens adultos interessados em gadgets');



INSERT INTO dbo.companies (user_id, company_name, description, sector, company_size, website_link, country_headquarters) 
VALUES (2, 'Tech Solutions', 'Agência de Marketing Digital', 'Publicidade', 50, 'https://techsolutions.com', 'Portugal');



INSERT INTO dbo.creator_social_profiles (creator_id, platform_id, platform_user_name, link, followers_count, price_per_post) 
VALUES (1, 2, 'joaovlogs_oficial', 'https://instagram.com/joaovlogs_oficial', 15000, 250.00);



INSERT INTO dbo.token (token_validation, created_at, last_used_at, user_id) 
VALUES ('abc-123-token-uuid', 1714560000000, 1714560000000, 1);