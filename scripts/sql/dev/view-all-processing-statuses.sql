-- This script displays a table of all processing and verification statuses
-- for all tournaments, matches, and scores. Useful for inspecting how the
-- DataWorkerService is behaving

SELECT
    'tournaments' AS source_table,
    CASE verification_status
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'PreRejected'
        WHEN 2 THEN 'PreVerified'
        WHEN 3 THEN 'Rejected'
        WHEN 4 THEN 'Verified'
        END AS verification_status,
    CASE processing_status
        WHEN 0 THEN 'NeedsApproval'
        WHEN 1 THEN 'NeedsMatchData'
        WHEN 2 THEN 'NeedsAutomationChecks'
        WHEN 3 THEN 'NeedsVerification'
        WHEN 4 THEN 'NeedsStatCalculation'
        WHEN 5 THEN 'Done'
        END AS processing_status,
    COUNT(*) AS count
FROM
    tournaments
GROUP BY
    processing_status, verification_status

UNION ALL

SELECT
    'matches' AS source_table,
    CASE verification_status
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'PreRejected'
        WHEN 2 THEN 'PreVerified'
        WHEN 3 THEN 'Rejected'
        WHEN 4 THEN 'Verified'
        END AS verification_status,
    CASE processing_status
        WHEN 0 THEN 'NeedsData'
        WHEN 1 THEN 'NeedsAutomationChecks'
        WHEN 2 THEN 'NeedsVerification'
        WHEN 3 THEN 'NeedsStatCalculation'
        WHEN 4 THEN 'NeedsRatingProcessorData'
        WHEN 5 THEN 'Done'
        END AS processing_status,
    COUNT(*) AS count
FROM
    matches
GROUP BY
    processing_status, verification_status

UNION ALL

SELECT
    'games' AS source_table,
    CASE verification_status
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'PreRejected'
        WHEN 2 THEN 'PreVerified'
        WHEN 3 THEN 'Rejected'
        WHEN 4 THEN 'Verified'
        END AS verification_status,
    CASE processing_status
        WHEN 0 THEN 'NeedsAutomationChecks'
        WHEN 1 THEN 'NeedsVerification'
        WHEN 2 THEN 'NeedsStatCalculation'
        WHEN 3 THEN 'Done'
        END AS processing_status,
    COUNT(*) AS count
FROM
    games
GROUP BY
    processing_status, verification_status

ORDER BY
    source_table, verification_status, processing_status;