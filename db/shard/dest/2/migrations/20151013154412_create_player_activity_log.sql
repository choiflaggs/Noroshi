
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `player_activity_daily_log` (
  `PlayerID` int unsigned NOT NULL,
  `StaminaConsuming` int unsigned NOT NULL,
  `BPConsuming` int unsigned NOT NULL,
  `CreatedOn` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`CreatedOn`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `player_activity_daily_log`;

