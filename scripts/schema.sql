create table players
(
    id            serial
        constraint "Player_pk"
            primary key,
    osu_id        bigint                              not null
        constraint "Players_osuid"
            unique,
    created       timestamp default CURRENT_TIMESTAMP not null,
    rank_standard integer,
    rank_taiko    integer,
    rank_catch    integer,
    rank_mania    integer,
    updated       timestamp,
    username      text
);

create table users
(
    id                 serial
        constraint "User_pk"
            primary key,
    player_id          integer                             not null
        constraint "Users___fkplayerid"
            references players,
    last_login         timestamp,
    created            timestamp default CURRENT_TIMESTAMP not null,
    roles              text,
    session_token      text,
    updated            timestamp,
    session_expiration timestamp
);

comment on column users.roles is 'Comma-delimited list of roles (e.g. user, admin, etc.)';

create table ratings
(
    id        serial
        constraint "Ratings_pk"
            primary key,
    player_id integer                             not null
        constraint "Ratings___fkplayerid"
            references players,
    mu        double precision                    not null,
    sigma     double precision                    not null,
    created   timestamp default CURRENT_TIMESTAMP not null,
    updated   timestamp,
    mode      integer                             not null,
    constraint ratings_playerid_mode
        unique (player_id, mode)
);

create table config
(
    key     text not null,
    value   text not null,
    id      serial,
    created timestamp
);

create table logs
(
    message          text,
    message_template text,
    level            integer,
    timestamp        timestamp,
    exception        text,
    log_event        jsonb
);

create table matches
(
    id                  integer   default nextval('osumatches_id_seq'::regclass) not null
        constraint matches_pk
            primary key,
    match_id            bigint                                                   not null
        constraint osumatches_matchid
            unique,
    name                text,
    start_time          timestamp,
    created             timestamp default CURRENT_TIMESTAMP                      not null,
    updated             timestamp,
    end_time            timestamp,
    verification_info   text,
    verification_source integer,
    verification_status integer
);

create table ratinghistories
(
    id        serial
        constraint "RatingHistories_pk"
            primary key,
    player_id integer                             not null
        constraint "RatingHistories___fkplayerid"
            references players,
    mu        double precision                    not null,
    sigma     double precision                    not null,
    created   timestamp default CURRENT_TIMESTAMP not null,
    mode      integer                             not null,
    match_id  integer                             not null
        constraint ratinghistories_matches_id_fk
            references matches,
    updated   timestamp
);

create table beatmaps
(
    id            integer   default nextval('beatmap_id_seq'::regclass) not null
        constraint beatmaps_pk
            primary key,
    artist        text                                                  not null,
    beatmap_id    bigint                                                not null
        constraint beatmaps_beatmapid
            unique,
    bpm           double precision                                      not null,
    mapper_id     bigint                                                not null,
    mapper_name   text                                                  not null,
    sr            double precision                                      not null,
    aim_diff      double precision,
    speed_diff    double precision,
    cs            double precision                                      not null,
    ar            double precision                                      not null,
    hp            double precision                                      not null,
    od            double precision                                      not null,
    drain_time    double precision                                      not null,
    length        double precision                                      not null,
    title         text                                                  not null,
    diff_name     text,
    game_mode     integer                                               not null,
    circle_count  integer                                               not null,
    slider_count  integer                                               not null,
    spinner_count integer                                               not null,
    max_combo     integer                                               not null,
    created       timestamp default CURRENT_TIMESTAMP                   not null
);

create table games
(
    id           integer   default nextval('osugames_id_seq'::regclass) not null
        constraint osugames_pk
            primary key,
    match_id     integer                                                not null
        constraint games_matches_id_fk
            references matches,
    beatmap_id   integer
        constraint games_beatmaps_id_fk
            references beatmaps,
    play_mode    integer                                                not null,
    match_type   integer                                                not null,
    scoring_type integer                                                not null,
    team_type    integer                                                not null,
    mods         integer                                                not null,
    game_id      bigint                                                 not null
        constraint osugames_gameid
            unique,
    created      timestamp default CURRENT_TIMESTAMP                    not null,
    start_time   timestamp                                              not null,
    end_time     timestamp,
    updated      timestamp
);

create table match_scores
(
    id           integer default nextval('scores_id_seq'::regclass) not null
        constraint match_scores_pk
            primary key,
    game_id      integer                                            not null
        constraint match_scores_games_id_fk
            references games,
    team         integer                                            not null,
    score        bigint                                             not null,
    max_combo    integer                                            not null,
    count_50     integer                                            not null,
    count_100    integer                                            not null,
    count_300    integer                                            not null,
    count_miss   integer                                            not null,
    perfect      boolean                                            not null,
    pass         boolean                                            not null,
    enabled_mods integer,
    count_katu   integer                                            not null,
    count_geki   integer                                            not null,
    player_id    integer                                            not null
        constraint match_scores_players_id_fk
            references players,
    constraint match_scores_gameid_playerid
        unique (game_id, player_id)
);

