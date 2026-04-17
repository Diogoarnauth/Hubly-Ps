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
('João Criador', 'joao@hubly.com', 'hash_password_123', true, 1714560000)
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
    ('Empresa Global', 'contact@empresa.com',   'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact1@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact2@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact3@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact4@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact5@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact6@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact7@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact8@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact9@empresa.com',  'hash_password_456', true, 1714560000),
    ('Empresa Global', 'contact10@empresa.com', 'hash_password_456', true, 1714560000);



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
    (13, 'AnaLifestyle',      true,  'AVAILABLE', 4.8, 12, 0, 0),
    (14, 'CarlosGadgets',     false, 'AVAILABLE', 4.5, 8,  0, 0),
    (15, 'MartaTrends',       true,  'AVAILABLE', 4.9, 25, 0, 0),
    (16, 'RikyPlay',          false, 'AVAILABLE', 4.2, 40, 0, 0),
    (17, 'SofiaZen',          true,  'AVAILABLE', 5.0, 15, 0, 0),
    (18, 'PedroVoyage',       false, 'AVAILABLE', 4.6, 20, 0, 0),
    (19, 'BeaGlow',           true,  'AVAILABLE', 4.7, 33, 0, 0),
    (20, 'AndreWeb3',         false, 'AVAILABLE', 4.4, 10, 0, 0),
    (21, 'CatTaste',          true,  'AVAILABLE', 4.9, 50, 0, 0),
    (22, 'DiogoRider',        false, 'AVAILABLE', 4.3, 18, 0, 0),
    (23, 'ElenaSustainable',  true,  'AVAILABLE', 4.8, 22, 0, 0),
    (24, 'FranStack',         false, 'AVAILABLE', 4.7, 14, 0, 0),
    (25, 'InesLiving',        true,  'AVAILABLE', 4.6, 19, 0, 0),
    (26, 'GoncaloPaws',       false, 'AVAILABLE', 4.9, 28, 0, 0),
    (27, 'LauraTeaching',     true,  'AVAILABLE', 4.5, 30, 0, 0),
    (28, 'MiguelPerformance', false, 'AVAILABLE', 4.8, 45, 0, 0),
    (29, 'RitaDaily',         true,  'AVAILABLE', 4.4, 60, 0, 0),
    (30, 'TiagoReview',       false, 'AVAILABLE', 4.7, 21, 0, 0),
    (31, 'VeraDigital',       true,  'AVAILABLE', 4.9, 35, 0, 0),
    (32, 'NunoSound',         false, 'AVAILABLE', 4.6, 17, 0, 0);


-- =============================================================================
-- 6. SETORES
-- =============================================================================
... (180 linhas)