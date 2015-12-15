
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `dynamic_text` (
  `ID` varchar(64) NOT NULL,
  `LanguageID` int unsigned NOT NULL,
  `Text` text NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `i1` (`LanguageID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `sequential_id_present_box` (
  `ID` int unsigned NOT NULL
) ENGINE=MyISAM;
INSERT INTO sequential_id_present_box VALUES (0);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `dynamic_text`;
DROP TABLE `sequential_id_present_box`;

