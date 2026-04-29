-- =============================================================================
-- 1. TABELAS DE DOMÍNIO (PLATAFORMAS)
-- =============================================================================

INSERT INTO dbo.social_platforms (name_platform) VALUES ('YouTube');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Instagram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Facebook');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('X');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Telegram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('TikTok');

-- =============================================================================
-- 2. UTILIZADORES (CREATORS)
-- =============================================================================

INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) VALUES 
('João Criador', 'joao@hubly.com', 'hash_password_123', true, 1714560000),
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

-- =============================================================================
-- 3. UTILIZADORES: COMPANIES
-- =============================================================================

INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES 
    ('NeuroLogic AI', 'contact@empresa.com',   'hash_password_456', true, 1714560000),
    ('Aura Atelier', 'contact1@empresa.com',  'hash_password_456', true, 1714560000),
    ('ChainVault', 'contact2@empresa.com',  'hash_password_456', true, 1714560000),
    ('Rooted Bites', 'contact3@empresa.com',  'hash_password_456', true, 1714560000),
    ('Pixel Horizon', 'contact4@empresa.com',  'hash_password_456', true, 1714560000),
    ('EcoFlow Solar', 'contact5@empresa.com',  'hash_password_456', true, 1714560000),
    ('HappyPaws Clinic', 'contact6@empresa.com',  'hash_password_456', true, 1714560000),
    ('RankMaster', 'contact7@empresa.com',  'hash_password_456', true, 1714560000),
    ('PeakQuest', 'contact8@empresa.com',  'hash_password_456', true, 1714560000),
    ('CodeForge Academy', 'contact9@empresa.com',  'hash_password_456', true, 1714560000),
    ('SkyRent Digital', 'contact10@empresa.com', 'hash_password_456', true, 1714560000);

    INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
    VALUES 
    ('Grip Performance', 'contact11@empresa.com', 'hash_password_456', true, 1714560000),
    ('Urban Nest', 'contact12@empresa.com', 'hash_password_456', true, 1714560000),
    ('Little Wonders', 'contact13@empresa.com', 'hash_password_456', true, 1714560000),
    ('SoundWave Events', 'contact14@empresa.com', 'hash_password_456', true, 1714560000),
    ('Velocity Motors', 'contact15@empresa.com', 'hash_password_456', true, 1714560000),
    ('BioHacker Labs', 'contact16@empresa.com', 'hash_password_456', true, 1714560000),
    ('Glow Up Retail', 'contact17@empresa.com', 'hash_password_456', true, 1714560000),
    ('Oceanic Travel', 'contact18@empresa.com', 'hash_password_456', true, 1714560000),
    ('GameChanger Agency', 'contact19@empresa.com', 'hash_password_456', true, 1714560000);



-- =============================================================================
-- 4. CONFIRMAÇÕES DE EMAIL
-- =============================================================================
INSERT INTO dbo.email_confirmation (user_id, confirmation_code, created_at, expires_at, used)
VALUES 
    (1, '123456', 1714560000, 1714646400, true),
    (2, '123456', 1714560000, 1714646400, true);


-- =============================================================================
-- 5. CREATORS PERFILS
-- =============================================================================

INSERT INTO dbo.creators 
    (user_id, artistic_name, is_verified, availability_status, global_rating, ratings_count, chats_started_count, chats_responded_count) 
VALUES 
    (1,  'JoaoVlogs',         false, 'AVAILABLE', 0.0, 0,  0, 0),
    (2, 'AnaLifestyle',      true,  'AVAILABLE', 4.8, 12, 0, 0),
    (3, 'CarlosGadgets',     false, 'AVAILABLE', 4.5, 8,  0, 0),
    (4, 'MartaTrends',       true,  'AVAILABLE', 4.9, 25, 0, 0),
    (5, 'RikyPlay',          false, 'AVAILABLE', 4.2, 40, 0, 0),
    (6, 'SofiaZen',          true,  'AVAILABLE', 5.0, 15, 0, 0),
    (7, 'PedroVoyage',       false, 'AVAILABLE', 4.6, 20, 0, 0),
    (8, 'BeaGlow',           true,  'AVAILABLE', 4.7, 33, 0, 0),
    (9, 'AndreWeb3',         false, 'AVAILABLE', 4.4, 10, 0, 0),
    (10, 'CatTaste',          true,  'AVAILABLE', 4.9, 50, 0, 0),
    (11, 'DiogoRider',        false, 'AVAILABLE', 4.3, 18, 0, 0),
    (12, 'ElenaSustainable',  true,  'AVAILABLE', 4.8, 22, 0, 0),
    (13, 'FranStack',         false, 'AVAILABLE', 4.7, 14, 0, 0),
    (14, 'InesLiving',        true,  'AVAILABLE', 4.6, 19, 0, 0),
    (15, 'GoncaloPaws',       false, 'AVAILABLE', 4.9, 28, 0, 0),
    (16, 'LauraTeaching',     true,  'AVAILABLE', 4.5, 30, 0, 0),
    (17, 'MiguelPerformance', false, 'AVAILABLE', 4.8, 45, 0, 0),
    (18, 'RitaDaily',         true,  'AVAILABLE', 4.4, 60, 0, 0),
    (19, 'TiagoReview',       false, 'AVAILABLE', 4.7, 21, 0, 0),
    (20, 'VeraDigital',       true,  'AVAILABLE', 4.9, 35, 0, 0),
    (21, 'NunoSound',         false, 'AVAILABLE', 4.6, 17, 0, 0);


-- =============================================================================
-- 6. SETORES
-- =============================================================================

INSERT INTO dbo.sectors (sector_name) VALUES 
('Technology & SaaS'), ('Fashion & Accessories'), ('Beauty & Personal Care'),
('Health & Wellness'), ('Finance & Fintech'), ('Education & E-learning'),
('Food & Beverages'), ('Travel & Tourism'), ('Gaming & E-sports'),
('Home & Decor'), ('Entertainment & Media'), ('Automotive & Mobility'),
('Sports & Fitness'), ('Real Estate'), ('Sustainability & Ecology'),
('Children & Maternity'), ('Pets'), ('Marketing & Advertising'),
('Retail & E-commerce'), ('Events & Lifestyle');


-- =============================================================================
-- 7. COMPANIES
-- =============================================================================

INSERT INTO dbo.companies (user_id, company_name, description, company_size, website_link, country_headquarters) VALUES 
(22, 'NeuroLogic AI', 'Soluções de IA.', '100 a 1000', 'https://neurologic.ai', 'Portugal'),
(23, 'Aura Atelier', 'Alta costura.', '0 a 100', 'https://aura-atelier.fr', 'France'),
(24, 'ChainVault', 'Web3 e Crypto.', '100 a 1000', 'https://chainvault.io', 'Switzerland'),
(25, 'Rooted Bites', 'Snacks vegetais.', '0 a 100', 'https://rootedbites.pt', 'Portugal'),
(26, 'Pixel Horizon', 'Jogos indie.', '0 a 100', 'https://pixelhorizon.games', 'Spain'),
(27, 'EcoFlow Solar', 'Painéis solares.', '100 a 1000', 'https://ecoflow.com', 'Germany'),
(28, 'HappyPaws Clinic', 'Veterinária.', '1000 a 1000', 'https://happypaws.com', 'UK'),
(29, 'RankMaster', 'SEO e Tráfego.', '0 a 100', 'https://rankmaster.net', 'USA'),
(30, 'PeakQuest', 'Trekking.', '0 a 100', 'https://peakquest.com', 'Nepal'),
(31, 'CodeForge Academy', 'Bootcamps.', '0 a 100', 'https://codeforge.edu', 'Portugal'),
(32, 'SkyRent Digital', 'PropTech.', '100 a 1000', 'https://skyrent.io', 'Italy');

INSERT INTO dbo.companies (user_id, company_name, description, company_size, website_link, country_headquarters) VALUES 
(33, 'Grip Performance', 'Equipamento desportivo de alta gama.', '100 a 1000', 'https://gripperformance.com', 'Germany'),
(34, 'Urban Nest', 'Consultoria imobiliária moderna.', '0 a 100', 'https://urbannest.pt', 'Portugal'),
(35, 'Little Wonders', 'Brinquedos educativos e sustentáveis.', '0 a 100', 'https://littlewonders.com', 'Denmark'),
(36, 'SoundWave Events', 'Gestão de festivais e concertos.', '100 a 1000', 'https://soundwave.events', 'UK'),
(37, 'Velocity Motors', 'Stand de veículos elétricos.', '0 a 100', 'https://velocity.motors', 'Spain'),
(38, 'BioHacker Labs', 'Suplementação e Biohacking.', '0 a 100', 'https://biohackerlabs.com', 'USA'),
(39, 'Glow Up Retail', 'Marketplace de beleza e skincare.', '100 a 1000', 'https://glowup.com', 'France'),
(40, 'Oceanic Travel', 'Cruzeiros e viagens de luxo.', '100 a 1000', 'https://oceanic.travel', 'Greece'),
(41, 'GameChanger Agency', 'Agência de Talentos para E-sports.', '0 a 100', 'https://gamechanger.io', 'Portugal');

-- =============================================================================
-- 8. RELAÇÃO: EMPRESAS E SETORES (COMPANY SECTORS)
-- =============================================================================

INSERT INTO dbo.company_sectors (company_user_id, sector_id) VALUES 
    (22, 1),   -- Technology & SaaS
    (22, 6),   -- Education & E-learning (talvez façam formação em IA)
    (23, 2),   -- Fashion & Accessories (Aura Atelier)
    (23, 19),  -- Retail & E-commerce
    (25, 7),   -- Food & Beverages (Rooted Bites)
    (25, 4),   -- Health & Wellness
    (25, 15),  -- Sustainability & Ecology
    (26, 9),   -- Pixel Horizon -> Gaming
    (27, 15),  -- EcoFlow -> Sustainability
    (28, 17),  -- HappyPaws -> Pets
    (29, 18),  -- RankMaster -> Marketing
    (30, 8),  -- PeakQuest -> Travel
    (31, 6),  -- CodeForge -> Education
    (32, 14); -- SkyRent -> Real Estate

    INSERT INTO dbo.company_sectors (company_user_id, sector_id) VALUES 
    (33, 13), -- Grip Performance -> Sports & Fitness
    (34, 14), -- Urban Nest -> Real Estate
    (35, 16), -- Little Wonders -> Children & Maternity
    (35, 15), -- Little Wonders -> Sustainability & Ecology
    (36, 11), -- SoundWave -> Entertainment & Media
    (36, 20), -- SoundWave -> Events & Lifestyle
    (37, 12), -- Velocity Motors -> Automotive
    (38, 4),  -- BioHacker Labs -> Health & Wellness
    (39, 3),  -- Glow Up -> Beauty & Personal Care
    (39, 19), -- Glow Up -> Retail & E-commerce
    (40, 8),  -- Oceanic Travel -> Travel & Tourism
    (41, 9),  -- GameChanger -> Gaming & E-sports
    (41, 18); -- GameChanger -> Marketing & Advertising


    
-- =============================================================================
-- 9. PERFIS SOCIAIS DOS CRIADORES (CREATOR SOCIAL PROFILES)
-- =============================================================================

INSERT INTO dbo.creator_social_profiles 
    (creator_id, platform_id, platform_user_name, link, description, followers_count, price_min, price_max) 
VALUES 
    (1,  2, 'joaovlogs_oficial',    'https://instagram.com/joaovlogs_oficial', 'Olá sou o joaovlgos e engano pessoas no casino', 15000, 10.00, 40.00),
    (2, 2, 'ana_silva_style',      'https://instagram.com/ana_silva_style',   'Dicas de lifestyle e organização diária.', 25000, 50.00, 150.00),
    (3, 1, 'CarlosGadgets',        'https://youtube.com/carlosgadgets',       'Reviews honestas de smartphones e setups.', 120000, 200.00, 500.00),
    (4, 6, 'martatrends_tok',      'https://tiktok.com/@martatrends_tok',     'Fashion hauls e tendências de moda rápida.', 350000, 150.00, 400.00),
    (5, 1, 'RikyPlayGames',        'https://youtube.com/rikyplay',            'Livestreams diárias de jogos competitivos.', 85000, 100.00, 300.00),
    (6, 2, 'sofia.zen.yoga',       'https://instagram.com/sofia.zen.yoga',    'Yoga, meditação e saúde mental.', 42000, 80.00, 200.00),
    (7, 2, 'pedro.voyage',         'https://instagram.com/pedro.voyage',      'A explorar o mundo com uma mochila às costas.', 67000, 120.00, 350.00),
    (8, 6, 'beaglow_makeup',       'https://tiktok.com/@beaglow_makeup',      'Tutoriais de maquilhagem para iniciantes.', 180000, 90.00, 250.00),
    (9, 1, 'AndreCryptoNews',      'https://youtube.com/andrecrypto',         'Análise de mercado e tecnologia blockchain.', 30000, 300.00, 800.00),
    (10, 2, 'cat_chef_taste',       'https://instagram.com/cat_chef_taste',    'Receitas saudáveis em menos de 15 minutos.', 95000, 150.00, 450.00),
    (11, 1, 'DiogoRiderVlogs',      'https://youtube.com/diogorider',          'Aventuras sobre duas rodas e manutenção.', 45000, 100.00, 280.00),
    (12, 6, 'elena_eco_living',     'https://tiktok.com/@elena_eco_living',    'Dicas para uma vida desperdício zero.', 110000, 70.00, 200.00),
    (13, 1, 'FranStackDev',         'https://youtube.com/franstack',           'Aulas de Fullstack e carreira em tecnologia.', 55000, 250.00, 600.00),
    (14, 2, 'ines_home_decor',      'https://instagram.com/ines_home_decor',   'Transformação de interiores com baixo orçamento.', 88000, 110.00, 320.00),
    (15, 6, 'goncalo_paws',         'https://tiktok.com/@goncalo_paws',        'O dia a dia dos meus 3 Golden Retrievers.', 210000, 130.00, 300.00),
    (16, 1, 'LauraLearnEnglish',    'https://youtube.com/lauralearn',          'Aprende inglês de forma prática e divertida.', 140000, 180.00, 450.00),
    (17, 2, 'miguel_perf_coach',    'https://instagram.com/miguel_perf_coach', 'Treino de alta performance e suplementação.', 72000, 200.00, 500.00),
    (18, 6, 'ritadaily_vlogs',      'https://tiktok.com/@ritadaily_vlogs',     'POV: A minha vida em Lisboa.', 500000, 250.00, 700.00),
    (19, 1, 'TiagoCinemaReview',    'https://youtube.com/tiagocinema',         'Críticas de filmes e séries do momento.', 38000, 90.00, 240.00),
    (20, 2, 'vera_digital_mkt',     'https://instagram.com/vera_digital_mkt',  'Estratégias de marketing para pequenos negócios.', 29000, 150.00, 400.00),
    (21, 1, 'NunoSoundProduction',  'https://youtube.com/nunosound',           'Como produzir música em casa.', 15000, 120.00, 300.00);


   
-- =============================================================================
-- 10. TOKENS
-- =============================================================================

INSERT INTO dbo.token (token_validation, created_at, last_used_at, user_id) 
VALUES ('abc-123-token-uuid', 1714560000000, 1714560000000, 1);

-- =============================================================================
-- 11. RELAÇÃO: PERFIS DE CRIADORES E SETORES (CREATOR PROFILE SECTORS)
-- =============================================================================
INSERT INTO dbo.creator_profile_sectors (profile_id, sector_id) VALUES 
    (1, 11), (1, 20), -- João (Entertainment & Events)
    (2, 20), (3, 1),  (4, 2),  (5, 9),  (6, 4), 
    (7, 8),  (8, 3),  (9, 5),  (10, 7), (11, 12),
    (12, 15), (13, 1), (14, 10), (15, 17), (16, 6),
    (17, 13), (18, 20), (19, 11), (20, 18), (21, 11);


-- =============================================================================
-- 12. HISTÓRICO DE VISUALIZAÇÕES (MÉTRICAS DE POPULARIDADE)
-- =============================================================================

INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 12, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
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
SELECT 2, 11, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 14) n;

-- 6º Lugar: BeaGlow (User ID 19) - 13 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 19, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 13) n;

-- 7º Lugar: GoncaloPaws (User ID 26) - 12 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 6, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
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
SELECT 2, 3, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 7) n;

-- 13º Lugar: LauraTeaching (User ID 27) - 6 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 7, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 6) n;

-- 14º Lugar: MiguelPerformance (User ID 28) - 5 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 8, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 5) n;

-- 15º Lugar: InesLiving (User ID 25) - 4 visualizações
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
SELECT 2, 5, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;

-- EXTRA: Criador João (ID 1) - Apenas 1 view (não deve aparecer no top 15 se houver mais gente)
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_creator_id, viewed_at)
VALUES (2, 1, CURRENT_TIMESTAMP);



-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 22, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 23, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 24, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 25, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 26, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 27, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 28, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 29, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 30, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 31, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


-- Companies History
INSERT INTO dbo.profile_views_history (viewer_user_id, viewed_company_id, viewed_at)
SELECT 5, 32, CURRENT_TIMESTAMP - (n || ' minutes')::interval 
FROM generate_series(1, 4) n;


