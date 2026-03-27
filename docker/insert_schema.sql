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

INSERT INTO dbo.sectors (sector_name) VALUES 
('Technology & SaaS'),
('Fashion & Accessories'),
('Beauty & Personal Care'),
('Health & Wellness'),
('Finance & Fintech'),
('Education & E-learning'),
('Food & Beverages'),
('Travel & Tourism'),
('Gaming & E-sports'),
('Home & Decor'),
('Entertainment & Media'),
('Automotive & Mobility'),
('Sports & Fitness'),
('Real Estate'),
('Sustainability & Ecology'),
('Children & Maternity'),
('Pets'),
('Marketing & Advertising'),
('Retail & E-commerce'),
('Events & Lifestyle');

-- 1. Tecnologia e SaaS
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(1, 'Artificial Intelligence'),
(1, 'Management Software (ERP/CRM)'),
(1, 'Cybersecurity'),
(1, 'Mobile Applications'),
(1, 'Cloud Computing');

-- 2. Fashion & Accessories
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(2, 'Streetwear'),
(2, 'Luxury Fashion'),
(2, 'Jewelry & Watches'),
(2, 'Athletic Footwear'),
(2, 'Sustainable Fashion');

-- 3. Beauty & Personal Care
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(3, 'Skincare (Facial Care)'),
(3, 'Professional Makeup'),
(3, 'Haircare'),
(3, 'Perfumery'),
(3, 'Natural Cosmetics');

-- 4. Health & Wellness
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(4, 'Dietary Supplements'),
(4, 'Mental Health & Meditation'),
(4, 'Home Medical Equipment'),
(4, 'Sports Nutrition'),
(4, 'Specialized Clinics');

-- 5. Finance & Fintech
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(5, 'Digital Banking & Neobanks'),
(5, 'Cryptocurrency & Blockchain'),
(5, 'Investments & Brokerage'),
(5, 'Online Insurance (Insurtech)'),
(5, 'Personal Finance Management');

-- 6. Education & E-learning
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(6, 'Language Courses'),
(6, 'Professional Training & Tech Bootcamps'),
(6, 'Tutoring & Academic Support'),
(6, 'Personal Development & Soft Skills'),
(6, 'EdTech Platforms');

-- 7. Food & Beverages
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(7, 'Restaurants & Delivery'),
(7, 'Supplements & Superfoods'),
(7, 'Craft Beverages & Wines'),
(7, 'Vegan & Vegetarian Food'),
(7, 'Snacks & Confectionery');

-- 8. Travel & Tourism
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(8, 'Local Accommodation & Hotels'),
(8, 'Online Travel Agencies'),
(8, 'Adventure Tourism & Experiences'),
(8, 'Transport & Vehicle Rental'),
(8, 'Ecotourism & Rural Tourism');

-- 9. Gaming & E-sports
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(9, 'Game Development (Indie/AAA)'),
(9, 'Gaming Equipment & Peripherals'),
(9, 'E-sports Tournament Organization'),
(9, 'Streaming & Content Platforms'),
(9, 'Mobile Gaming');

-- 10. Home & Decor
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(10, 'Furniture & Interior Design'),
(10, 'Home Automation & Smart Home'),
(10, 'Textiles & Bedding'),
(10, 'Decorative Lighting'),
(10, 'DIY & Garden');

-- 11. Entertainment & Media
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(11, 'Video Streaming & TV'),
(11, 'Podcasts & Audio Content'),
(11, 'Event & Show Production'),
(11, 'News & Digital Portals'),
(11, 'Cinema & Animation');

-- 12. Automotive & Mobility
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(12, 'Electric & Hybrid Vehicles'),
(12, 'Auto Parts & Accessories'),
(12, 'Sharing Services (Scooters/Cars)'),
(12, 'Digital Workshops & Maintenance'),
(12, 'Urban Micro-mobility');

-- 13. Sports & Fitness
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(13, 'Gyms & Health Clubs'),
(13, 'Home Workout Equipment'),
(13, 'Sportswear (Activewear)'),
(13, 'Sporting Events & Marathons'),
(13, 'Yoga & Pilates');

-- 14. Real Estate
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(14, 'Real Estate Brokerage (Buy/Sell)'),
(14, 'Short & Long Term Rentals'),
(14, 'Digital Property Management'),
(14, 'Real Estate Investment & Flipping'),
(14, 'Architecture & Renovation');

-- 15. Sustainability & Ecology
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(15, 'Renewable Energy & Solar'),
(15, 'Waste Management & Recycling'),
(15, 'Zero Waste & Organic Products'),
(15, 'Environmental Consulting'),
(15, 'Circular & Second-hand Fashion');

-- 16. Children & Maternity
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(16, 'Educational Toys'),
(16, 'Children’s Clothing'),
(16, 'Baby Nutrition & Food'),
(16, 'Nursery Furniture'),
(16, 'Parenting Support Services');

-- 17. Pets
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(17, 'Premium Pet Food'),
(17, 'Pet Accessories & Toys'),
(17, 'Health & Veterinary Clinics'),
(17, 'Pet Sitting & Dog Walking Services'),
(17, 'Animal Hygiene & Grooming');

-- 18. Marketing & Advertising
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(18, 'Digital Marketing Agencies'),
(18, 'Social Media & Influencer Management'),
(18, 'Branding & Graphic Design'),
(18, 'SEO & Paid Traffic'),
(18, 'Content & Video Creation');

-- 19. Retail & E-commerce
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(19, 'Marketplaces & Multi-brand Stores'),
(19, 'E-commerce Logistics & Shipping'),
(19, 'Online Payment Systems'),
(19, 'Dropshipping Solutions'),
(19, 'Customer Experience & Chatbots');

-- 20. Events & Lifestyle
INSERT INTO dbo.sub_sectors (sector_id, subsector_name) VALUES 
(20, 'Wedding & Party Planning'),
(20, 'Luxury Experiences & Concierge'),
(20, 'Event Photography & Videography'),
(20, 'Event Catering & Gastronomy'),
(20, 'Workshops & Leisure Experiences');
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (2, 'NeuroLogic AI', 'Soluções de inteligência artificial para análise preditiva.', 1, 1, '100 a 1000', 'https://neurologic.ai', 'Portugal');

-- Company 2: Fashion (Luxury) - Boutique exclusiva
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (3, 'Aura Atelier', 'Alta costura e acessórios de luxo feitos à mão.', 2, 7, '0 a 100', 'https://aura-atelier.fr', 'France');

-- Company 3: Finance (Crypto) - Fintech disruptiva
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (4, 'ChainVault', 'Plataforma segura para gestão de ativos digitais e Web3.', 5, 22, '100 a 1000', 'https://chainvault.io', 'Switzerland');

-- Company 4: Food & Beverages (Vegan) - Marca de retalho
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (5, 'Rooted Bites', 'Snacks 100% vegetais e biológicos para o dia-a-dia.', 7, 34, '0 a 100', 'https://rootedbites.pt', 'Portugal');

-- Company 5: Gaming (Indie Dev) - Estúdio criativo
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (6, 'Pixel Horizon', 'Desenvolvimento de jogos indie focados em narrativa.', 9, 41, '0 a 100', 'https://pixelhorizon.games', 'Spain');

-- Company 6: Sustainability (Energy) - Grande empresa
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (7, 'EcoFlow Solar', 'Instalação e manutenção de painéis solares residenciais.', 15, 71, '100 a 1000', 'https://ecoflow.com', 'Germany');

-- Company 7: Pets (Health) - Rede de clínicas
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (8, 'HappyPaws Clinic', 'Rede de cuidados veterinários e bem-estar animal.', 17, 83, '1000 a 1000', 'https://happypaws.com', 'United Kingdom');

-- Company 8: Marketing (SEO) - Agência especializada
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (9, 'RankMaster', 'Especialistas em tráfego pago e posicionamento orgânico.', 18, 89, '0 a 100', 'https://rankmaster.net', 'USA');

-- Company 9: Travel (Adventure) - Agência de nicho
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (10, 'PeakQuest', 'Experiências de trekking e turismo de aventura radical.', 8, 38, '0 a 100', 'https://peakquest.com', 'Nepal');

-- Company 10: Education (Bootcamps) - EdTech
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (11, 'CodeForge Academy', 'Bootcamps intensivos de programação e UX Design.', 6, 27, '0 a 100', 'https://codeforge.edu', 'Portugal');

-- Company 11: Real Estate (Management) - PropTech
INSERT INTO dbo.companies (user_id, company_name, description, sector_id, sub_sector_id, company_size, website_link, country_headquarters) 
VALUES (12, 'SkyRent Digital', 'Gestão automatizada de arrendamentos de curta duração.', 14, 68, '100 a 1000', 'https://skyrent.io', 'Italy');



INSERT INTO dbo.creator_social_profiles (creator_id, platform_id, platform_user_name, link, followers_count, price_per_post) 
VALUES (1, 2, 'joaovlogs_oficial', 'https://instagram.com/joaovlogs_oficial', 15000, 250.00);


INSERT INTO dbo.token (token_validation, created_at, last_used_at, user_id) 
VALUES ('abc-123-token-uuid', 1714560000000, 1714560000000, 1);