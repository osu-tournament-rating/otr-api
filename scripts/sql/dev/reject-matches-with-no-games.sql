-- Set the verification status to Rejected
-- and processing status to Done for all matches
-- which do not have any games

UPDATE matches SET processing_status = 5, 
    verification_status = 3 WHERE id IN (SELECT matches.id FROM matches
        WHERE NOT EXISTS (SELECT 1 FROM games
            WHERE games.match_id = matches.id));