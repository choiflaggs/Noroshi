
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `release_level`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

CREATE TABLE `release_level` (
  `ID` int(10) unsigned NOT NULL,
  `ContentsID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

