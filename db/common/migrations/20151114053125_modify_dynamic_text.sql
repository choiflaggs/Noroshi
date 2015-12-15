
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `dynamic_text`;
CREATE TABLE `dynamic_text` (
  `LanguageID` int unsigned NOT NULL,
  `TextKey` varchar(32) NOT NULL,
  `Text` text NOT NULL,
  PRIMARY KEY (`LanguageID`,`TextKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `dynamic_text`;
CREATE TABLE `dynamic_text` (
  `ID` varchar(64) NOT NULL,
  `LanguageID` int(10) unsigned NOT NULL,
  `Text` text NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `i1` (`LanguageID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

