UPDATE tournaments SET rejection_reason = 0; -- None
UPDATE tournaments SET verification_status = 0; -- None
UPDATE tournaments SET processing_status = 2; -- Needs automation checks

UPDATE matches SET rejection_reason = 0; -- None
UPDATE matches SET verification_status = 0; -- None
UPDATE matches SET processing_status = 1; -- Needs automation checks

UPDATE games SET rejection_reason = 0; -- None
UPDATE games SET verification_status = 0; -- None
UPDATE games SET processing_status = 0; -- Needs automation checks

UPDATE game_scores SET rejection_reason = 0; -- None
UPDATE game_scores SET verification_status = 0; -- None
UPDATE game_scores SET processing_status = 0; -- Needs automation checks