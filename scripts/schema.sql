create table players
(
    id      serial
        constraint "Player_pk"
            primary key,
    osu_id  bigint                              not null
        constraint "Players_osuid"
            unique,
    created timestamp default CURRENT_TIMESTAMP not null
);

create table users
(
    id         serial
        constraint "User_pk"
            primary key,
    player_id  integer                             not null
        constraint "Users___fkplayerid"
            references players,
    last_login timestamp                           not null,
    created    timestamp default CURRENT_TIMESTAMP not null
);

create table playermatchdata
(
    id         integer default nextval('matchdata_id_seq'::regclass) not null
        constraint "MatchData_pk"
            primary key,
    player_id  integer                                               not null
        constraint "MatchData___fkplayerid"
            references players,
    match_id   integer                                               not null,
    beatmap_id integer                                               not null
);

comment on column playermatchdata.match_id is 'The ID of the match from public.osumatches';

comment on column playermatchdata.beatmap_id is 'ID of the beatmap from public.beatmaps';

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
    mode      text                                not null,
    constraint ratings_playerid_mode
        unique (player_id, mode)
);

create index "Ratings__mu"
    on ratings (mu desc, mu desc);

create table ratinghistories
(
    id            serial
        constraint "RatingHistories_pk"
            primary key,
    player_id     integer                             not null
        constraint "RatingHistories___fkplayerid"
            references players,
    mu            double precision                    not null,
    sigma         double precision                    not null,
    created       timestamp default CURRENT_TIMESTAMP not null,
    mode          text                                not null,
    match_data_id integer                             not null
        constraint "RatingHistories___fkmatchdataid"
            references playermatchdata
);

create table config
(
    key   text not null,
    value text not null
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
    id                  integer   default nextval('osumatches_id_seq'::regclass) not null,
    match_id            bigint                                                   not null
        constraint osumatches_matchid
            unique,
    name                text                                                     not null,
    start_time          timestamp                                                not null,
    created             timestamp default CURRENT_TIMESTAMP                      not null,
    updated             timestamp,
    end_time            timestamp,
    verification_info   text,
    verification_source integer,
    verification_status integer
);

create table games
(
    id           integer   default nextval('osugames_id_seq'::regclass) not null
        constraint osugames_pk
            primary key,
    match_id     integer                                                not null,
    beatmap_id   integer,
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
    end_time     timestamp
);

create table scores
(
    id           serial
        constraint scores_pk
            primary key,
    match_id     integer not null,
    team         integer not null,
    score        bigint  not null,
    max_combo    integer not null,
    count_50     integer not null,
    count_100    integer not null,
    count_300    integer not null,
    count_miss   integer not null,
    perfect      boolean not null,
    pass         boolean not null,
    enabled_mods integer not null,
    count_katu   integer not null,
    count_geki   integer not null
);

create table beatmap
(
    id            serial,
    artist        text                                not null,
    beatmap_id    bigint                              not null,
    bpm           double precision                    not null,
    mapper_id     bigint                              not null,
    mapper_name   bigint                              not null,
    sr            double precision                    not null,
    aim_diff      double precision                    not null,
    speed_diff    double precision                    not null,
    cs            double precision                    not null,
    ar            double precision                    not null,
    hp            double precision                    not null,
    od            double precision                    not null,
    drain_time    double precision                    not null,
    total_length  double precision                    not null,
    title         text                                not null,
    diff_name     text,
    game_mode     integer                             not null,
    circle_count  integer                             not null,
    slider_count  integer                             not null,
    spinner_count integer                             not null,
    max_combo     integer                             not null,
    created       timestamp default CURRENT_TIMESTAMP not null
);