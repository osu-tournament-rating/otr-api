UPDATE tournaments SET verification_status = 4 WHERE verification_status = 2;
UPDATE matches SET verification_status = 4 WHERE verification_status = 2;
UPDATE games SET verification_status = 4 WHERE verification_status = 2;
UPDATE game_scores SET verification_status = 4 WHERE verification_status = 2;

UPDATE tournaments SET verification_status = 3 WHERE verification_status = 1;
UPDATE matches SET verification_status = 3 WHERE verification_status = 1;
UPDATE games SET verification_status = 3 WHERE verification_status = 1;
UPDATE game_scores SET verification_status = 3 WHERE verification_status = 1;