
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `ranking_reward` (
  `RankingID` int unsigned NOT NULL,
  `ThresholdRank` int unsigned NOT NULL,
  `No` tinyint unsigned NOT NULL,
  `PossessionCategory` tinyint unsigned NOT NULL,
  `PossessionID` int unsigned NOT NULL,
  `PossessionNum` int unsigned NOT NULL,
  PRIMARY KEY (`RankingID`,`ThresholdRank`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `ranking_reward`;

