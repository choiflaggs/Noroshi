
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `player_counter` (
  `ID` int unsigned NOT NULL,
  `Count` int unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `player_counter`;

