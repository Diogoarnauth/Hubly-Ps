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

CREATE TABLE IF NOT EXISTS dbo.sectors (
    id SERIAL PRIMARY KEY,
    sector_name VARCHAR(100) NOT NULL UNIQUE 
);

CREATE TABLE IF NOT EXISTS dbo.companies (
    user_id INTEGER PRIMARY KEY REFERENCES dbo.users(id) ON DELETE CASCADE,
    company_name VARCHAR(150) NOT NULL UNIQUE,
    is_verified BOOLEAN DEFAULT false,
    description TEXT,
    company_size VARCHAR(100),    
    website_link VARCHAR(100),
    country_headquarters VARCHAR(100)
);

CREATE TABLE IF NOT EXISTS dbo.company_sectors (
    company_user_id INTEGER NOT NULL REFERENCES dbo.companies(user_id) ON DELETE CASCADE,
    sector_id INTEGER NOT NULL REFERENCES dbo.sectors(id) ON DELETE CASCADE,
    PRIMARY KEY (company_user_id, sector_id)
);

CREATE TABLE IF NOT EXISTS dbo.creator_social_profiles (
    id SERIAL PRIMARY KEY,
    creator_id INTEGER NOT NULL REFERENCES dbo.creators(user_id) ON DELETE CASCADE,
    platform_id INTEGER NOT NULL REFERENCES dbo.social_platforms(id) ON DELETE CASCADE,
    platform_user_name VARCHAR(100) NOT NULL, 
    link VARCHAR(255),
    description TEXT,
    followers_count INTEGER DEFAULT 0 CHECK (followers_count >= 0),
    price_min DECIMAL DEFAULT NULL,
    price_max DECIMAL DEFAULT NULL,
    CONSTRAINT unique_platform_username UNIQUE(platform_id, platform_user_name)
);

CREATE TABLE IF NOT EXISTS dbo.creator_profile_sectors (
    profile_id INTEGER NOT NULL REFERENCES dbo.creator_social_profiles(id) ON DELETE CASCADE,
    sector_id INTEGER NOT NULL REFERENCES dbo.sectors(id) ON DELETE CASCADE,
    PRIMARY KEY (profile_id, sector_id)
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

CREATE TABLE IF NOT EXISTS dbo.profile_views_history (
    id SERIAL PRIMARY KEY,
    viewer_user_id INTEGER NOT NULL REFERENCES dbo.users(id),
    viewed_company_id INTEGER REFERENCES dbo.companies(user_id),
    viewed_social_profile_id INTEGER REFERENCES dbo.creator_social_profiles(id), 
    viewed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT chk_only_one_viewed CHECK (
        (viewed_company_id IS NOT NULL AND viewed_social_profile_id IS NULL) OR 
        (viewed_company_id IS NULL AND viewed_social_profile_id IS NOT NULL)
    )
);

CREATE TABLE IF NOT EXISTS dbo.creator_ratings (
    id SERIAL PRIMARY KEY,
    evaluator_id INTEGER NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE,
    target_creator_id INTEGER NOT NULL REFERENCES dbo.creators(user_id) ON DELETE CASCADE,
    rating_value INTEGER NOT NULL CHECK (rating_value >= 1 AND rating_value <= 5),
    rated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT unique_user_rating UNIQUE(evaluator_id, target_creator_id),
    
    CONSTRAINT chk_not_self_rating CHECK (evaluator_id <> target_creator_id)
);

CREATE TABLE IF NOT EXISTS dbo.conversations (
    id SERIAL PRIMARY KEY,
    created_at BIGINT NOT NULL,
    last_message_at BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS dbo.conversation_participants (
    conversation_id INTEGER NOT NULL REFERENCES dbo.conversations(id) ON DELETE CASCADE,
    user_id INTEGER NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE,
    company_id INTEGER REFERENCES dbo.companies(user_id),
    social_profile_id INTEGER REFERENCES dbo.creator_social_profiles(id),
    CONSTRAINT chk_participant_role CHECK (
        (company_id IS NOT NULL AND social_profile_id IS NULL) OR
        (company_id IS NULL AND social_profile_id IS NOT NULL)
    ),

    PRIMARY KEY (conversation_id, user_id)
);

CREATE TABLE IF NOT EXISTS dbo.messages (
    id SERIAL PRIMARY KEY,
    conversation_id INTEGER NOT NULL REFERENCES dbo.conversations(id) ON DELETE CASCADE,
    sender_id INTEGER NOT NULL REFERENCES dbo.users(id), 
    content TEXT NOT NULL,
    sent_at BIGINT NOT NULL,
    is_edited BOOLEAN DEFAULT false,
    is_deleted BOOLEAN DEFAULT false
);

CREATE TABLE IF NOT EXISTS dbo.message_read_status (
    conversation_id INTEGER NOT NULL REFERENCES dbo.conversations(id) ON DELETE CASCADE,
    user_id INTEGER NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE,
    last_read_message_id INTEGER REFERENCES dbo.messages(id) ON DELETE SET NULL,
    last_read_at BIGINT,
    PRIMARY KEY (conversation_id, user_id)
);

CREATE TABLE IF NOT EXISTS dbo.conversation_tags (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES dbo.users(id) ON DELETE CASCADE, 
    tag_name VARCHAR(50) NOT NULL,
    color_hex VARCHAR(7) DEFAULT '#808080', 
    created_at BIGINT NOT NULL,

    CONSTRAINT unique_user_tag_name UNIQUE(user_id, tag_name)
);

CREATE TABLE IF NOT EXISTS dbo.conversation_tag_assignments (
    user_id INTEGER NOT NULL REFERENCES dbo.users(id) ON DELETE CASCADE,
    conversation_id INTEGER NOT NULL REFERENCES dbo.conversations(id) ON DELETE CASCADE,
    tag_id INTEGER NOT NULL REFERENCES dbo.conversation_tags(id) ON DELETE CASCADE,

    PRIMARY KEY (user_id, conversation_id), 
    updated_at BIGINT NOT NULL
);