# Seeding

* **Name**: The name of the game you are adding to the SUGAR system. Please note that this name must be unique to the host.

## Achievements/Skills

* **Name**: The name of the achievement/skill you are creating. Please note that this name must be unique to the game. 
* **Description**: The description of the achievement/skill you are creating.
* **Token**: The unique identification token for the achievement/skill you are creating. Please note that this name must be unique to the game. 
* **[ActorType](#actortype)**: The type of Actor which this achievement/skill applies to.

#### Evaluation Criteria

* **EvaluationDataKey**: The key of the data which is being used for this criteria. 
* **[EvaluationDataCategory](#evaluationdatacategory)**: The category of data which the criteria is being checked against. 
* **[ComparisonType](#comparisontype)**: What the current value has to be compared to the target value in order for this criteria to be completed.
* **[CriteriaQueryType](#criteriaquerytype)**: The data set from which the current value will be gathered.
* **[EvaluationDataType](#evaluationdatatype)**: The type of data which the criteria is being checked for.
* **[Scope](#criteriascope)**: The range of actors from which the data will be collected for comparision.
* **Value**: The target value of this criteria.

#### Rewards

* **EvaluationDataKey**: The key of the data which will be provided to the actor upon completion of all criteria.
* **[EvaluationDataCategory](#evaluationdatacategory)**: The category of data which will be provided to the actor upon completion of all criteria.
* **[EvaluationDataType](#evaluationdatatype)**: The type of data which will be provided to the actor upon completion of all criteria.
* **Value**: The value which will be provided to the actor upon completion of all criteria.

## Leaderboards

* **Token**: The unique identification token for the leaderboard you are creating. Please note that this name must be unique to the game. 
* **Name**: The name of the leaderboard you are creating. Please note that this name must be unique to the game. 
* **Key**: The key of the data which will be used to form the leaderboard.
* **[ActorType](#actortype)**: The type of Actor which this leaderboard applies to.
* **[EvaluationDataCategory](#evaluationdatacategory)**: The category of data which will be used for this leaderboard.
* **[EvaluationDataType](#evaluationdatatype)**: The type of data which will be used for this leaderboard.
* **[CriteriaScope](#criteriascope)**: The range of actors from which the data will be collected.
* **[LeaderboardType](#leaderboardtype)**: How collected data will be sorted for this leaderboard.

## Enums

#### ActorType

* **Undefined**: Intended use not defined and as such means it applies to both Users and Groups.
* **User**: Intended for use for Users only.
* **Group**: Intended for use for Groups only.

#### ComparisonType

* **Equals**: The current value and target value must exactly match.
* **Not Equal**: The current value and target value do not exactly match.
* **Greater**: The current value is greater in value than the target value (Long and Float only).
* **Greater or Equal**: The current value is greater or exactly equal in value to the target value (Long and Float only).
* **Lesser**: The current value is lower in value than the target value (Long and Float only).
* **Lesser or Equal**: The current value is lower or exactly equal in value to the target value (Long and Float only).

#### CriteriaScope

* **Actor**: Data collected will have been submitted by the actor themselves.
* **Related Users**: Data collected will have been submitted by the actor and their friends (if the actor is a User) or their members (if the actor is a Group). ActorType cannot also be Undefined.
* **Related Groups**: Data collected will have been submitted by the actor and their alliances. ActorType must be Group.
* **Related Group Users**: Data collected will have been submitted by their members and the members of their alliances. ActorType must be Group.

#### CriteriaQueryType

* **Any**: Any collected data will be compared against the value set in the criteria. Can only be used when Scope is set to Actor.
* **Sum**: The sum of all collected data will be compared against the value set in the criteria. Can only be used when EvaluationDataType is set to Long or Float.
* **Latest**: The latest piece of data will be compared against the value set in the criteria. Can only be used when Scope is set to Actor.

#### EvaluationDataCategory

* **Game Data**: The data has been stored as a piece of Game Data, which allows for multiple values for the same key and all EvaluationDataTypes.
* **Resource**: The data has been stored as a Resource, which allows for only one value per key and has its value stored as a Long.
* **Skill**: The data has been stored as a Skill, marking the completion of all criteria for a Skill.
* **Achievement**: The data has been stored as a Achievement, marking the completion of all criteria for a Achievement.
* **Match Data**: The data has been stored as a piece of Match Data, which allows for multiple values for the same key in relation to a Match and all EvaluationDataTypes.

#### EvaluationDataType

* **String**: The data has been stored and can be parsed as a String.
* **Long**: The data has been stored and can be parsed as a Long.
* **Float**: The data has been stored and can be parsed as a Float.
* **Boolean**: The data has been stored and can be parsed as a Boolean.

#### LeaderboardType

* **Highest**: The leaderboard will be sorted from highest single value to lowest (Long and Float only).
* **Lowest**: The leaderboard will be sorted from lowest single value to highest (Long and Float only).
* **Cumulative**: The leaderboard will be sorted from highest combined value to lowest (Long and Float only).
* **Count**: The leaderboard will be sorted from the highest amount which the key has been recorded to the lowest (String and Boolean only).
* **Earliest**: The leaderboard will be sorted from the earliest time the key was recorded to the latest (String and Boolean only).
* **Latest**: The leaderboard will be sorted from the latest time the key was recorded to the earliest (String and Boolean only).