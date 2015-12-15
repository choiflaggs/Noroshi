
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `player_cooperate_character`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

CREATE TABLE `player_cooperate_character` (
  `PlayerID` int(10) unsigned NOT NULL,
  `CooperateID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`CooperateID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

