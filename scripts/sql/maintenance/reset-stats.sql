-- Updates all verified tournaments, matches, and games such that the
-- DataWorkerService will re-generate stats for them

UPDATE tournaments SET processing_status = 4 WHERE verification_status = 4 AND processing_status > 4;
UPDATE matches SET processing_status = 3 WHERE verification_status = 4 AND processing_status > 3;
UPDATE games SET processing_status = 2 WHERE verification_status = 4 AND processing_status > 2;