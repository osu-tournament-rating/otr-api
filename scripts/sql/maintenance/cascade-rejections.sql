-- Cascade rejections from parent entities to child entities
-- If a parent entity is rejected, reject all children with the existing rejection reason 
-- plus the numeric equivalent of the appropriate RejectedParent flag

BEGIN;

-- Step 1: Reject matches whose tournaments are rejected
-- Add MatchRejectionReason.RejectedTournament (128) to existing rejection reasons
UPDATE matches 
SET rejection_reason = matches.rejection_reason | 128, verification_status = 3
FROM tournaments t
WHERE matches.tournament_id = t.id 
  AND t.rejection_reason = 3
  AND (matches.rejection_reason & 128) = 0;

-- Step 2: Reject games whose matches are rejected  
-- Add GameRejectionReason.RejectedMatch (512) to existing rejection reasons
UPDATE games 
SET rejection_reason = games.rejection_reason | 512, verification_status = 3
FROM matches m
WHERE games.match_id = m.id 
  AND m.rejection_reason = 3
  AND (games.rejection_reason & 512) = 0;

-- Step 3: Reject scores whose games are rejected
-- Add ScoreRejectionReason.RejectedGame (8) to existing rejection reasons  
UPDATE game_scores 
SET rejection_reason = game_scores.rejection_reason | 8, verification_status = 3
FROM games g
WHERE game_scores.game_id = g.id 
  AND g.rejection_reason = 3
  AND (game_scores.rejection_reason & 8) = 0;

COMMIT;