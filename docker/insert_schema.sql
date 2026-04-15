INSERT INTO dbo.social_platforms (name_platform) VALUES ('YouTube');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Instagram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Facebook');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('X');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Telegram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('TikTok');

--user -> creator 1
INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('João Criador', 'joao@hubly.com', 'hash_password_123', true, 1714560000);

INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) VALUES 
('Ana Silva', 'ana.silva@hubly.com', 'hash_ana', true, 1714560000),
('Carlos Tech', 'carlos.m@hubly.com', 'hash_carlos', true, 1714560000),
('Marta Fashion', 'marta.style@hubly.com', 'hash_marta', true, 1714560000),
('Ricardo Gamer', 'riky.gamer@hubly.com', 'hash_ricardo', true, 1714560000),
('Sofia Wellness', 'sofia.fit@hubly.com', 'hash_sofia', true, 1714560000),
('Pedro Viajante', 'pedro.world@hubly.com', 'hash_pedro', true, 1714560000),
('Beatriz MakeUp', 'bea.beauty@hubly.com', 'hash_beatriz', true, 1714560000),
('André Crypto', 'andre.fin@hubly.com', 'hash_andre', true, 1714560000),
('Catarina Foodie', 'cat.chef@hubly.com', 'hash_catarina', true, 1714560000),
('Diogo Moto', 'diogo.wheels@hubly.com', 'hash_diogo', true, 1714560000),
('Elena Eco', 'elena.green@hubly.com', 'hash_elena', true, 1714560000),
('Francisco Dev', 'fran.code@hubly.com', 'hash_fran', true, 1714560000),
('Inês Decor', 'ines.home@hubly.com', 'hash_ines', true, 1714560000),
('Gonçalo Pets', 'goncalo.dog@hubly.com', 'hash_goncalo', true, 1714560000),
('Laura Edu', 'laura.learn@hubly.com', 'hash_laura', true, 1714560000),
('Miguel Sports', 'miguel.atleta@hubly.com', 'hash_miguel', true, 1714560000),
('Rita Lifestyle', 'rita.vlogs@hubly.com', 'hash_rita', true, 1714560000),
('Tiago Cinema', 'tiago.movies@hubly.com', 'hash_tiago', true, 1714560000),
('Vera Marketing', 'vera.ads@hubly.com', 'hash_vera', true, 1714560000),
('Nuno Beats', 'nuno.music@hubly.com', 'hash_nuno', true, 1714560000);



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

INSERT INTO dbo.creators (user_id, artistic_name, is_verified, availability_status, global_rating, ratings_count) VALUES 
(13, 'AnaLifestyle', true, 'AVAILABLE', 4.8, 12),
(14, 'CarlosGadgets', false, 'AVAILABLE', 4.5, 8),
(15, 'MartaTrends', true, 'AVAILABLE', 4.9, 25),
(16, 'RikyPlay', false, 'AVAILABLE', 4.2, 40),
(17, 'SofiaZen', true, 'AVAILABLE', 5.0, 15),
(18, 'PedroVoyage', false, 'AVAILABLE', 4.6, 20),
(19, 'BeaGlow', true, 'AVAILABLE', 4.7, 33),
(20, 'AndreWeb3', false, 'AVAILABLE', 4.4, 10),
(21, 'CatTaste', true, 'AVAILABLE', 4.9, 50),
(22, 'DiogoRider', false, 'AVAILABLE', 4.3, 18),
(23, 'ElenaSustainable', true, 'AVAILABLE', 4.8, 22),
(24, 'FranStack', false, 'AVAILABLE', 4.7, 14),
(25, 'InesLiving', true, 'AVAILABLE', 4.6, 19),
(26, 'GoncaloPaws', false, 'AVAILABLE', 4.9, 28),
(27, 'LauraTeaching', true, 'AVAILABLE', 4.5, 30),
(28, 'MiguelPerformance', false, 'AVAILABLE', 4.8, 45),
(29, 'RitaDaily', true, 'AVAILABLE', 4.4, 60),
(30, 'TiagoReview', false, 'AVAILABLE', 4.7, 21),
(31, 'VeraDigital', true, 'AVAILABLE', 4.9, 35),
(32, 'NunoSound', false, 'AVAILABLE', 4.6, 17);

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

INSERT INTO dbo.creator_social_profiles (creator_id, platform_id, platform_user_name, link, description, followers_count, price_min, price_max) VALUES 
(13, 2, 'ana_silva_style', 'https://instagram.com/ana_silva_style', 'Dicas de lifestyle e organização diária.', 25000, 50.00, 150.00),
(14, 1, 'CarlosGadgets', 'https://youtube.com/carlosgadgets', 'Reviews honestas de smartphones e setups.', 120000, 200.00, 500.00),
(15, 6, 'martatrends_tok', 'https://tiktok.com/@martatrends_tok', 'Fashion hauls e tendências de moda rápida.', 350000, 150.00, 400.00),
(16, 1, 'RikyPlayGames', 'https://youtube.com/rikyplay', 'Livestreams diárias de jogos competitivos.', 85000, 100.00, 300.00),
(17, 2, 'sofia.zen.yoga', 'https://instagram.com/sofia.zen.yoga', 'Yoga, meditação e saúde mental.', 42000, 80.00, 200.00),
(18, 2, 'pedro.voyage', 'https://instagram.com/pedro.voyage', 'A explorar o mundo com uma mochila às costas.', 67000, 120.00, 350.00),
(19, 6, 'beaglow_makeup', 'https://tiktok.com/@beaglow_makeup', 'Tutoriais de maquilhagem para iniciantes.', 180000, 90.00, 250.00),
(20, 1, 'AndreCryptoNews', 'https://youtube.com/andrecrypto', 'Análise de mercado e tecnologia blockchain.', 30000, 300.00, 800.00),
(21, 2, 'cat_chef_taste', 'https://instagram.com/cat_chef_taste', 'Receitas saudáveis em menos de 15 minutos.', 95000, 150.00, 450.00),
(22, 1, 'DiogoRiderVlogs', 'https://youtube.com/diogorider', 'Aventuras sobre duas rodas e manutenção.', 45000, 100.00, 280.00),
(23, 6, 'elena_eco_living', 'https://tiktok.com/@elena_eco_living', 'Dicas para uma vida desperdício zero.', 110000, 70.00, 200.00),
(24, 1, 'FranStackDev', 'https://youtube.com/franstack', 'Aulas de Fullstack e carreira em tecnologia.', 55000, 250.00, 600.00),
(25, 2, 'ines_home_decor', 'https://instagram.com/ines_home_decor', 'Transformação de interiores com baixo orçamento.', 88000, 110.00, 320.00),
(26, 6, 'goncalo_paws', 'https://tiktok.com/@goncalo_paws', 'O dia a dia dos meus 3 Golden Retrievers.', 210000, 130.00, 300.00),
(27, 1, 'LauraLearnEnglish', 'https://youtube.com/lauralearn', 'Aprende inglês de forma prática e divertida.', 140000, 180.00, 450.00),
(28, 2, 'miguel_perf_coach', 'https://instagram.com/miguel_perf_coach', 'Treino de alta performance e suplementação.', 72000, 200.00, 500.00),
(29, 6, 'ritadaily_vlogs', 'https://tiktok.com/@ritadaily_vlogs', 'POV: A minha vida em Lisboa.', 500000, 250.00, 700.00),
(30, 1, 'TiagoCinemaReview', 'https://youtube.com/tiagocinema', 'Críticas de filmes e séries do momento.', 38000, 90.00, 240.00),
(31, 2, 'vera_digital_mkt', 'https://instagram.com/vera_digital_mkt', 'Estratégias de marketing para pequenos negócios.', 29000, 150.00, 400.00),
(32, 1, 'NunoSoundProduction', 'https://youtube.com/nunosound', 'Como produzir música em casa.', 15000, 120.00, 300.00);

INSERT INTO dbo.token (token_validation, created_at, last_used_at, user_id) 
VALUES ('abc-123-token-uuid', 1714560000000, 1714560000000, 1);



INSERT INTO dbo.creator_profile_sectors (profile_id, sector_id) VALUES 
(1, 11), 
(1, 20);

INSERT INTO dbo.creator_profile_sectors (profile_id, sector_id) VALUES 
(2, 20), (3, 1), (4, 2), (5, 9), (6, 4), 
(7, 8), (8, 3), (9, 5), (10, 7), (11, 12),
(12, 15), (13, 1), (14, 10), (15, 17), (16, 6),
(17, 13), (18, 20), (19, 11), (20, 18), (21, 11);


INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 29, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 20) n;

-- 2º Lugar: MartaTrends (User ID 15) - 18 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 15, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 18) n;

-- 3º Lugar: CatTaste (User ID 21) - 16 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 21, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 16) n;

-- 4º Lugar: RikyPlay (User ID 16) - 15 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 16, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 15) n;

-- 5º Lugar: VeraDigital (User ID 31) - 14 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 31, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 14) n;

-- 6º Lugar: BeaGlow (User ID 19) - 13 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 19, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 13) n;

-- 7º Lugar: GoncaloPaws (User ID 26) - 12 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 26, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 12) n;

-- 8º Lugar: AnaLifestyle (User ID 13) - 11 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 13, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 11) n;

-- 9º Lugar: CarlosGadgets (User ID 14) - 10 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 14, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 10) n;

-- 10º Lugar: SofiaZen (User ID 17) - 9 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 17, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 9) n;

-- 11º Lugar: PedroVoyage (User ID 18) - 8 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 18, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 8) n;

-- 12º Lugar: ElenaSustainable (User ID 23) - 7 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 23, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 7) n;

-- 13º Lugar: LauraTeaching (User ID 27) - 6 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 27, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 6) n;

-- 14º Lugar: MiguelPerformance (User ID 28) - 5 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 28, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 5) n;

-- 15º Lugar: InesLiving (User ID 25) - 4 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 25, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;

-- EXTRA: Criador João (ID 1) - Apenas 1 view (não deve aparecer no top 15 se houver mais gente)
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
VALUES (2, 1, CURRENT_TIMESTAMP);