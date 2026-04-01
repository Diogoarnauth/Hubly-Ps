INSERT INTO dbo.social_platforms (name_platform) VALUES ('YouTube');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Instagram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Facebook');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('X');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Telegram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('TikTok');

--user -> creator 1
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('João Criador', 'joao@hubly.com', 'hash_password_123', true, 1714560000);



--user -> company 1
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 2
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact1@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 3
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact2@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 4
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact3@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 5
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact4@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 6
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact5@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 7
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact6@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 8
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact7@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 9
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact8@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 10
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact9@empresa.com', 'hash_password_456', true, 1714560000);

--user -> company 11
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact10@empresa.com', 'hash_password_456', true, 1714560000);

INSERT INTO dbo.email_confirmation (user_id, confirmation_code, created_at, expires_at, used)
VALUES (1, '123456', 1714560000, 1714646400, true);

INSERT INTO dbo.email_confirmation (user_id, confirmation_code, created_at, expires_at, used)
VALUES (2, '123456', 1714560000, 1714646400, true);

INSERT INTO dbo.creators (user_id, artistic_name, is_verified, availability_status, global_rating, ratings_count, chats_started_count, chats_responded_count) 
VALUES (1, 'JoaoVlogs', false, 'AVAILABLE', 0.0, 0, 0, 0);

-- 2. SETORES
INSERT INTO dbo.sectors (sector_name) VALUES 
('Technology & SaaS'), ('Fashion & Accessories'), ('Beauty & Personal Care'),
('Health & Wellness'), ('Finance & Fintech'), ('Education & E-learning'),
('Food & Beverages'), ('Travel & Tourism'), ('Gaming & E-sports'),
('Home & Decor'), ('Entertainment & Media'), ('Automotive & Mobility'),
('Sports & Fitness'), ('Real Estate'), ('Sustainability & Ecology'),
('Children & Maternity'), ('Pets'), ('Marketing & Advertising'),
('Retail & E-commerce'), ('Events & Lifestyle');

INSERT INTO dbo.companies (user_id, company_name, description, company_size, website_link, country_headquarters) VALUES 
(2, 'NeuroLogic AI', 'Soluções de IA.', '100 a 1000', 'https://neurologic.ai', 'Portugal'),
(3, 'Aura Atelier', 'Alta costura.', '0 a 100', 'https://aura-atelier.fr', 'France'),
(4, 'ChainVault', 'Web3 e Crypto.', '100 a 1000', 'https://chainvault.io', 'Switzerland'),
(5, 'Rooted Bites', 'Snacks vegetais.', '0 a 100', 'https://rootedbites.pt', 'Portugal'),
(6, 'Pixel Horizon', 'Jogos indie.', '0 a 100', 'https://pixelhorizon.games', 'Spain'),
(7, 'EcoFlow Solar', 'Painéis solares.', '100 a 1000', 'https://ecoflow.com', 'Germany'),
(8, 'HappyPaws Clinic', 'Veterinária.', '1000 a 1000', 'https://happypaws.com', 'UK'),
(9, 'RankMaster', 'SEO e Tráfego.', '0 a 100', 'https://rankmaster.net', 'USA'),
(10, 'PeakQuest', 'Trekking.', '0 a 100', 'https://peakquest.com', 'Nepal'),
(11, 'CodeForge Academy', 'Bootcamps.', '0 a 100', 'https://codeforge.edu', 'Portugal'),
(12, 'SkyRent Digital', 'PropTech.', '100 a 1000', 'https://skyrent.io', 'Italy');

INSERT INTO dbo.creator_sectors (creator_user_id, sector_id) VALUES 
(1, 11), (1, 20);

INSERT INTO dbo.company_sectors (company_user_id, sector_id) VALUES 
(2, 1),  -- Technology & SaaS
(2, 6);  -- Education & E-learning (talvez façam formação em IA)

-- Exemplo: Aura Atelier (ID 3) agora é Fashion (2) e também Retail & E-commerce (19)
INSERT INTO dbo.company_sectors (company_user_id, sector_id) VALUES 
(3, 2),  -- Fashion & Accessories
(3, 19); -- Retail & E-commerce (têm loja online própria)

-- Exemplo: Rooted Bites (ID 5) é Food (7), Health (4) e Sustainability (15)
INSERT INTO dbo.company_sectors (company_user_id, sector_id) VALUES 
(5, 7),  -- Food & Beverages
(5, 4),  -- Health & Wellness
(5, 15); -- Sustainability & Ecology

INSERT INTO dbo.company_sectors (company_user_id, sector_id) VALUES 
(6, 9),  -- Pixel Horizon -> Gaming
(7, 15), -- EcoFlow -> Sustainability
(8, 17), -- HappyPaws -> Pets
(9, 18), -- RankMaster -> Marketing
(10, 8), -- PeakQuest -> Travel
(11, 6), -- CodeForge -> Education
(12, 14);-- SkyRent -> Real Estate

INSERT INTO dbo.creator_social_profiles (creator_id, platform_id, platform_user_name, link, description,followers_count, price_min, price_max) 
VALUES (1, 2, 'joaovlogs_oficial', 'https://instagram.com/joaovlogs_oficial','Olá sou o joaovlgos e engano pessoas no casino', 15000, 10.00, 40.00 );


INSERT INTO dbo.token (token_validation, created_at, last_used_at, user_id) 
VALUES ('abc-123-token-uuid', 1714560000000, 1714560000000, 1);