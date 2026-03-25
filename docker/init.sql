CREATE SCHEMA IF NOT EXISTS dbo;

CREATE TABLE IF NOT EXISTS dbo.social_platforms (
    id SERIAL PRIMARY KEY,
    name_platform VARCHAR(50) NOT NULL UNIQUE
);


CREATE TABLE IF NOT EXISTS dbo.users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    password_validation VARCHAR(255) NOT NULL,
    is_email_confirmed boolean not null default false,
    created_at BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS dbo.token (
    token_validation VARCHAR(255) PRIMARY KEY,
    created_at BIGINT NOT NULL,
    last_used_at BIGINT NOT NULL,
    user_id INTEGER NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS dbo.creators (
    user_id INTEGER PRIMARY KEY REFERENCES dbo.users(id) ON DELETE CASCADE,
    artistic_name VARCHAR(100) NOT NULL,
    is_verified BOOLEAN DEFAULT false,
    availability_status VARCHAR(20) DEFAULT 'AVAILABLE',
    global_rating DECIMAL(3, 2) DEFAULT 0, 
    ratings_count INTEGER DEFAULT 0,      
    chats_started_count INTEGER DEFAULT 0,   
    chats_responded_count INTEGER DEFAULT 0 
);

CREATE TABLE IF NOT EXISTS dbo.companies (
    user_id INTEGER PRIMARY KEY REFERENCES dbo.users(id) ON DELETE CASCADE,
    company_name VARCHAR(150) NOT NULL,
    is_verified BOOLEAN DEFAULT false,
    description TEXT,
    sector VARCHAR(100),
    company_size VARCHAR(100),    
    website_link VARCHAR(100),
    country_headquarters VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS dbo.creator_social_profiles (
    id SERIAL PRIMARY KEY,
    creator_id INTEGER NOT NULL REFERENCES dbo.creators(user_id) ON DELETE CASCADE,
    platform_id INTEGER NOT NULL REFERENCES dbo.social_platforms(id) ON DELETE CASCADE,
    platform_user_name VARCHAR(100), 
    link VARCHAR(255),
    followers_count INTEGER DEFAULT 0 CHECK (followers_count >= 0),
    price_per_post DECIMAL(10, 2) DEFAULT 0.00 CHECK (price_per_post >= 0),
    CONSTRAINT unique_creator_platform UNIQUE(creator_id, platform_id)
);


CREATE TABLE IF NOT EXISTS dbo.email_confirmation (
    id serial primary key,
    user_id integer not null,
    confirmation_code varchar(255) not null,
    created_at bigint not null,
    expires_at bigint not null,
    used boolean not null default false,
    foreign key (user_id) references dbo.users(id) on delete cascade
);