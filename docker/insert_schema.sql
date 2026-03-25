
INSERT INTO dbo.social_platforms (name_platform) VALUES ('YouTube');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Instagram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Facebook');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('X');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('Telegram');
INSERT INTO dbo.social_platforms (name_platform) VALUES ('TikTok');

INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('João Criador', 'joao@hubly.com', 'hash_password_123', true, 1714560000);

INSERT INTO dbo.users (name, email, password_validation, is_email_confirmed, created_at) 
VALUES ('Empresa Global', 'contact@empresa.com', 'hash_password_456', true, 1714560000);


INSERT INTO dbo.email_confirmation (user_id, confirmation_code, created_at, expires_at, used)
VALUES (1, '123456', 1714560000, 1714646400, true);

INSERT INTO dbo.email_confirmation (user_id, confirmation_code, created_at, expires_at, used)
VALUES (2, '123456', 1714560000, 1714646400, true);

INSERT INTO dbo.creators (user_id, artistic_name, is_verified, availability_status, global_rating, ratings_count, chats_started_count, chats_responded_count) 
VALUES (1, 'JoaoVlogs', false, 'AVAILABLE', 0.0, 0, 0, 0);

INSERT INTO dbo.sectors (name) VALUES 
('Tecnologia e SaaS'),
('Moda e Acessórios'),
('Beleza e Cuidados Pessoais'),
('Saúde e Bem-estar'),
('Finanças e Fintech'),
('Educação e E-learning'),
('Alimentação e Bebidas'),
('Viagens e Turismo'),
('Gaming e E-sports'),
('Casa e Decoração'),
('Entretenimento e Media'),
('Automóvel e Mobilidade'),
('Desporto e Fitness'),
('Imobiliário'),
('Sustentabilidade e Ecologia'),
('Crianças e Maternidade'),
('Animais de Estimação (Pets)'),
('Marketing e Publicidade'),
('Retalho e E-commerce'),
('Eventos e Lifestyle');

-- 1. Tecnologia e SaaS
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(1, 'Inteligência Artificial'),
(1, 'Software de Gestão (ERP/CRM)'),
(1, 'Cibersegurança'),
(1, 'Aplicações Móveis'),
(1, 'Cloud Computing');

-- 2. Moda e Acessórios
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(2, 'Streetwear'),
(2, 'Moda de Luxo'),
(2, 'Joalharia e Relógios'),
(2, 'Calçado Desportivo'),
(2, 'Moda Sustentável');

-- 3. Beleza e Cuidados Pessoais
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(3, 'Skincare (Cuidados do Rosto)'),
(3, 'Maquilhagem Profissional'),
(3, 'Cuidados Capilares'),
(3, 'Perfumaria'),
(3, 'Cosmética Natural');

-- 4. Saúde e Bem-estar
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(4, 'Suplementos Alimentares'),
(4, 'Saúde Mental e Meditação'),
(4, 'Equipamento Médico Doméstico'),
(4, 'Nutrição Desportiva'),
(4, 'Clínicas Especializadas');

-- 5. Finanças e Fintech
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(5, 'Banca Digital e Neobanks'),
(5, 'Criptomoedas e Blockchain'),
(5, 'Investimentos e Corretoras'),
(5, 'Seguros Online (Insurtech)'),
(5, 'Gestão de Finanças Pessoais');

-- 6. Educação e E-learning
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(6, 'Cursos de Línguas'),
(6, 'Formação Profissional e Tech Bootcamps'),
(6, 'Apoio Escolar e Explicações'),
(6, 'Desenvolvimento Pessoal e Soft Skills'),
(6, 'Plataformas de EdTech');

-- 7. Alimentação e Bebidas
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(7, 'Restauração e Delivery'),
(7, 'Suplementação e Superalimentos'),
(7, 'Bebidas Artesanais e Vinhos'),
(7, 'Alimentação Vegan e Vegetariana'),
(7, 'Snacks e Confeitaria');

-- 8. Viagens e Turismo
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(8, 'Alojamento Local e Hotéis'),
(8, 'Agências de Viagens Online'),
(8, 'Turismo de Aventura e Experiências'),
(8, 'Transportes e Aluguer de Veículos'),
(8, 'Ecoturismo e Turismo Rural');

-- 9. Gaming e E-sports
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(9, 'Desenvolvimento de Jogos (Indie/AAA)'),
(9, 'Equipamento e Periféricos Gaming'),
(9, 'Organização de Torneios de E-sports'),
(9, 'Plataformas de Streaming e Conteúdo'),
(9, 'Mobile Gaming');

-- 10. Casa e Decoração
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(10, 'Mobiliário e Design de Interiores'),
(10, 'Domótica e Smart Home'),
(10, 'Têxteis e Enxoval'),
(10, 'Iluminação Decorativa'),
(10, 'Bricolage e Jardim');

-- 11. Entretenimento e Media
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(11, 'Streaming de Vídeo e TV'),
(11, 'Podcasts e Conteúdo Áudio'),
(11, 'Produção de Eventos e Espetáculos'),
(11, 'Notícias e Portais Digitais'),
(11, 'Cinema e Animação');

-- 12. Automóvel e Mobilidade
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(12, 'Veículos Elétricos e Híbridos'),
(12, 'Acessórios e Peças Auto'),
(12, 'Serviços de Sharing (Trotinetes/Carros)'),
(12, 'Oficinas e Manutenção Digital'),
(12, 'Micro-mobilidade Urbana');

-- 13. Desporto e Fitness
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(13, 'Ginasios e Health Clubs'),
(13, 'Equipamento de Treino em Casa'),
(13, 'Moda Desportiva (Activewear)'),
(13, 'Eventos Desportivos e Maratonas'),
(13, 'Yoga e Pilates');

-- 14. Imobiliário
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(14, 'Mediação Imobiliária (Compra/Venda)'),
(14, 'Arrendamento de Curta e Longa Duração'),
(14, 'Gestão de Condomínios Digital'),
(14, 'Investimento Imobiliário e Flipping'),
(14, 'Arquitetura e Reabilitação');

-- 15. Sustentabilidade e Ecologia
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(15, 'Energias Renováveis e Solar'),
(15, 'Gestão de Resíduos e Reciclagem'),
(15, 'Produtos Zero Waste e Bio'),
(15, 'Consultoria Ambiental'),
(15, 'Moda Circular e Segunda Mão');

-- 16. Crianças e Maternidade
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(16, 'Brinquedos Educativos'),
(16, 'Vestuário Infantil'),
(16, 'Nutrição e Papas de Bebé'),
(16, 'Mobiliário de Quarto de Bebé'),
(16, 'Serviços de Apoio à Parentalidade');

-- 17. Animais de Estimação (Pets)
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(17, 'Alimentação Premium para Pets'),
(17, 'Acessórios e Brinquedos para Animais'),
(17, 'Saúde e Clínicas Veterinárias'),
(17, 'Serviços de Pet Sitting e Dog Walking'),
(17, 'Higiene e Estética Animal (Grooming)');

-- 18. Marketing e Publicidade
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(18, 'Agências de Marketing Digital'),
(18, 'Gestão de Redes Sociais e Influencers'),
(18, 'Branding e Design Gráfico'),
(18, 'SEO e Tráfego Pago'),
(18, 'Criação de Conteúdo e Vídeo');

-- 19. Retalho e E-commerce
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(19, 'Marketplaces e Lojas Multimarca'),
(19, 'Logística e Envios para E-commerce'),
(19, 'Sistemas de Pagamento Online'),
(19, 'Soluções de Dropshipping'),
(19, 'Experiência de Cliente e Chatbots');

-- 20. Eventos e Lifestyle
INSERT INTO dbo.sub_sectors (sector_id, name) VALUES 
(20, 'Organização de Casamentos e Festas'),
(20, 'Experiências de Luxo e Concierge'),
(20, 'Fotografia e Videografia de Eventos'),
(20, 'Catering e Gastronomia de Eventos'),
(20, 'Workshops e Experiências de Lazer');

INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (2, 'Tech Solutions', 'Agência de Marketing Digital', 18, 86, '0 a 100', 'https://techsolutions.com', 'Portugal');

INSERT INTO dbo.creator_social_profiles (creator_id, platform_id, platform_user_name, link, followers_count, price_per_post) 
VALUES (1, 2, 'joaovlogs_oficial', 'https://instagram.com/joaovlogs_oficial', 15000, 250.00);


INSERT INTO dbo.token (token_validation, created_at, last_used_at, user_id) 
VALUES ('abc-123-token-uuid', 1714560000000, 1714560000000, 1);
