# How to Correctly Set Up a Destination Object

You can use empty GameObjects as destination objects if you simply wish to move your agent to a certain location.
If, however, your destination object has a collider on it, you need to add a Nav Mesh Obstacle component to it.
Make sure to set its Carve parameter to true and, if it is not a moving object, set Carve Only Stationary to true as well.