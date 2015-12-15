
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
CREATE TABLE `information` (
  `ID` int(10) unsigned NOT NULL,
  `TitleTextKey` varchar(32) NOT NULL,
  `BodyTextKey` varchar(32) NOT NULL,
  `BannerInformation` text NOT NULL,
  `Category` tinyint(3) unsigned NOT NULL,
  `OpenedAt` int(10) unsigned NOT NULL,
  `ClosedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Title_UNIQUE` (`TitleTextKey`),
  UNIQUE KEY `Body_UNIQUE` (`BodyTextKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
DROP TABLE `information`;
