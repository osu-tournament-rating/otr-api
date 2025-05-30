-- Reset automated checks for all tournaments, matches, games, and scores
-- such that the DataWorkerService will re-process all unverified data.

UPDATE tournaments
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 2
WHERE verification_status != 4 AND verification_status != 3;

UPDATE matches
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 1
WHERE verification_status != 4 AND verification_status != 3;

UPDATE games
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 0
WHERE verification_status != 4 AND verification_status != 3;

UPDATE game_scores
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 0
WHERE verification_status != 4 AND verification_status != 3;
