create table players
(
    id      serial                              not null
        constraint "Player_pk"
            primary key,
    osu_id  bigint                              not null
        constraint "Players_osuid"
            unique,
    created timestamp default CURRENT_TIMESTAMP not null
);

create table users
(
    id         serial                              not null
        constraint "User_pk"
            primary key,
    player_id  integer                             not null
        constraint "Users___fkplayerid"
            references players,
    last_login timestamp                           not null,
    created    timestamp default CURRENT_TIMESTAMP not null
);

create table matchdata
(
    id                  serial           not null
        constraint "MatchData_pk"
            primary key,
    player_id           integer          not null
        constraint "MatchData___fkplayerid"
            references players,
    osu_match_id        bigint           not null,
    game_id             integer          not null,
    scoring_type        text             not null,
    score               double precision not null,
    osu_beatmap_id      bigint           not null,
    game_raw_mods       integer,
    raw_mods            integer          not null,
    match_name          text             not null,
    mode                text             not null,
    match_start_date    timestamp        not null,
    created             timestamp        not null,
    free_mod            boolean default false,
    force_mod           boolean default false,
    team_type           text,
    team                text,
    osu_name            text,
    osu_rank            integer,
    osu_badges          integer          not null,
    osu_duel_starrating double precision,
    accuracy            double precision,
    cs                  double precision,
    ar                  double precision,
    od                  double precision
);

create table ratings
(
    id            serial                              not null
        constraint "Ratings_pk"
            primary key,
    player_id     integer                             not null
        constraint "Ratings___fkplayerid"
            references players,
    mu            double precision                    not null,
    sigma         double precision                    not null,
    created       timestamp default CURRENT_TIMESTAMP not null,
    updated       timestamp,
    mode          text                                not null,
    constraint ratings_playerid_mode
        unique (player_id, mode)
);

create index "Ratings__mu"
    on ratings (mu desc, mu desc);

create table ratinghistories
(
    id        serial                              not null
        constraint "RatingHistories_pk"
            primary key,
    player_id integer                             not null
        constraint "RatingHistories___fkplayerid"
            references players,
    mu        double precision                    not null,
    sigma     double precision                    not null,
    created   timestamp default CURRENT_TIMESTAMP not null,
    mode      text                                not null,
    match_data_id integer                         not null
        constraint "RatingHistories___fkmatchdataid"
            references matchdata
);

create table config
(
    key   text not null,
    value text not null
);

create table osumatches
(
    match_id   bigint                              not null
        constraint osumatches_pk
            primary key,
    name       text                                not null,
    start_time timestamp                           not null,
    created    timestamp default CURRENT_TIMESTAMP not null,
    updated    timestamp,
    end_time   timestamp                           not null
);
