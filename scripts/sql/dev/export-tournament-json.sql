-- Export Tournament Data to Nested JSON
-- 
-- This script exports a tournament and all its child data (matches, games, scores, players)
-- as a single nested JSON object.
--
-- MODIFY THE TOURNAMENT ID BELOW:
-- ================================
DO $$
DECLARE
    tournament_id_param INTEGER := 1;  -- << CHANGE THIS VALUE TO YOUR DESIRED TOURNAMENT ID
    result JSON;
BEGIN
    -- Build the nested JSON structure
    SELECT json_build_object(
        'id', t.id,
        'name', t.name,
        'abbreviation', t.abbreviation,
        'forum_url', t.forum_url,
        'rank_range_lower_bound', t.rank_range_lower_bound,
        'ruleset', t.ruleset,
        'lobby_size', t.lobby_size,
        'verification_status', t.verification_status,
        'last_processing_date', t.last_processing_date,
        'rejection_reason', t.rejection_reason,
        'processing_status', t.processing_status,
        'submitted_by_user_id', t.submitted_by_user_id,
        'verified_by_user_id', t.verified_by_user_id,
        'start_time', t.start_time,
        'end_time', t.end_time,
        'created', t.created,
        'updated', t.updated,
        'matches', COALESCE((
            SELECT json_agg(
                json_build_object(
                    'id', m.id,
                    'osu_id', m.osu_id,
                    'tournament_id', m.tournament_id,
                    'name', m.name,
                    'start_time', m.start_time,
                    'end_time', m.end_time,
                    'verification_status', m.verification_status,
                    'last_processing_date', m.last_processing_date,
                    'rejection_reason', m.rejection_reason,
                    'warning_flags', m.warning_flags,
                    'processing_status', m.processing_status,
                    'submitted_by_user_id', m.submitted_by_user_id,
                    'verified_by_user_id', m.verified_by_user_id,
                    'created', m.created,
                    'updated', m.updated,
                    'games', COALESCE((
                        SELECT json_agg(
                            json_build_object(
                                'id', g.id,
                                'osu_id', g.osu_id,
                                'match_id', g.match_id,
                                'beatmap_id', g.beatmap_id,
                                'ruleset', g.ruleset,
                                'scoring_type', g.scoring_type,
                                'team_type', g.team_type,
                                'mods', g.mods,
                                'start_time', g.start_time,
                                'end_time', g.end_time,
                                'verification_status', g.verification_status,
                                'rejection_reason', g.rejection_reason,
                                'warning_flags', g.warning_flags,
                                'processing_status', g.processing_status,
                                'last_processing_date', g.last_processing_date,
                                'play_mode', g.play_mode,
                                'created', g.created,
                                'updated', g.updated,
                                'scores', COALESCE((
                                    SELECT json_agg(
                                        json_build_object(
                                            'id', gs.id,
                                            'game_id', gs.game_id,
                                            'player_id', gs.player_id,
                                            'score', gs.score,
                                            'placement', gs.placement,
                                            'max_combo', gs.max_combo,
                                            'count50', gs.count50,
                                            'count100', gs.count100,
                                            'count300', gs.count300,
                                            'count_miss', gs.count_miss,
                                            'count_katu', gs.count_katu,
                                            'count_geki', gs.count_geki,
                                            'pass', gs.pass,
                                            'perfect', gs.perfect,
                                            'grade', gs.grade,
                                            'mods', gs.mods,
                                            'team', gs.team,
                                            'ruleset', gs.ruleset,
                                            'verification_status', gs.verification_status,
                                            'last_processing_date', gs.last_processing_date,
                                            'rejection_reason', gs.rejection_reason,
                                            'processing_status', gs.processing_status,
                                            'created', gs.created,
                                            'updated', gs.updated,
                                            'player', json_build_object(
                                                'id', p.id,
                                                'osu_id', p.osu_id,
                                                'username', p.username,
                                                'country', p.country,
                                                'default_ruleset', p.default_ruleset,
                                                'osu_last_fetch', p.osu_last_fetch,
                                                'osu_track_last_fetch', p.osu_track_last_fetch,
                                                'created', p.created,
                                                'updated', p.updated
                                            )
                                        )
                                        ORDER BY gs.placement, gs.id
                                    )
                                    FROM game_scores gs
                                    JOIN players p ON p.id = gs.player_id
                                    WHERE gs.game_id = g.id
                                ), '[]'::json)
                            )
                            ORDER BY g.start_time, g.id
                        )
                        FROM games g
                        WHERE g.match_id = m.id
                    ), '[]'::json)
                )
                ORDER BY m.start_time, m.id
            )
            FROM matches m
            WHERE m.tournament_id = t.id
        ), '[]'::json)
    ) INTO result
    FROM tournaments t
    WHERE t.id = tournament_id_param;
    
    -- Output the result
    IF result IS NULL THEN
        RAISE NOTICE 'No tournament found with ID %', tournament_id_param;
    ELSE
        -- Pretty print the JSON result
        RAISE NOTICE '%', jsonb_pretty(result::jsonb);
    END IF;
END $$;