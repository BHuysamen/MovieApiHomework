# MovieApiHomework
Homework Project

Mostly happy,  some additional refactoring could be done.  Some questions around expected usage of movies vs user updates.  I.E, does it make more sense to update a movies average rating on the movie record each time a user updates their rating or to query all the ratings each time we want to get the top five.

Not entirely happy with how I'm dealing with the Top Fives in the domain layer service class, but I'm tired and I don't have the brain power left to refactor it further.

Test coverage is not complete :-(
Started to get late and the choice was finish the API or stick strictly to TDD.

Getting movies with filters is implemented as a post.  I wanted to pass filters in the body but the standard doesn't allow a body with a get, hence the post.  As for why, I felt like it would be more easily extensible without having a massively unweildly url.
