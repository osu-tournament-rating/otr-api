-- Reset automated checks for all tournaments, matches, games, and scores
-- such that the DataWorkerService will re-process all unverified data.

BEGIN TRANSACTION;

UPDATE tournaments
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 2
WHERE verification_status NOT IN (4, 3);

UPDATE matches
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 1,
    warning_flags = 0
WHERE verification_status NOT IN (4, 3);

UPDATE games
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 0,
    warning_flags = 0
WHERE verification_status NOT IN (4, 3);

UPDATE game_scores
SET rejection_reason = 0,
    verification_status = 0,
    processing_status = 0
WHERE verification_status NOT IN (4, 3);

COMMIT;