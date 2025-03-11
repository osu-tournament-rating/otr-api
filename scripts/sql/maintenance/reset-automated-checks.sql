-- Update tournaments, matches, games, and game scores
-- such that the DataWorkerService will re-generate automated checks on unverified data.

UPDATE tournaments
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 2
WHERE verification_status != 4;

UPDATE matches
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 1
WHERE verification_status != 4;

UPDATE games
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 0
WHERE verification_status != 4;

UPDATE game_scores
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 0
WHERE verification_status != 4;
