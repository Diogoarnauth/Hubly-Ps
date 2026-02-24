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
    artistic_name VARCHAR(100),
    content TEXT,
    audience TEXT
);

CREATE TABLE IF NOT EXISTS dbo.companies (
    user_id INTEGER PRIMARY KEY REFERENCES dbo.users(id) ON DELETE CASCADE,
    company_name VARCHAR(150) NOT NULL,
    description TEXT,
    sector VARCHAR(100),
    company_size INTEGER,
    website_link VARCHAR(255),
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