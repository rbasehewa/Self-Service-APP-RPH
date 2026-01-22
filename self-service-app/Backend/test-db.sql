-- Connect to StaffServiceDB first, then run this

-- Check what tables exist
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public';

-- If Users table exists, count records
SELECT COUNT(*) as total_users FROM "Users";

-- If Users table exists, show first 5 users
SELECT * FROM "Users" LIMIT 5;
