# How to add new animations to the agent and integrate them into the existing infrastructure

1. Create a new empty state in your agent's Animator. Give it a usable name and assign the desired motion to it.
2. Add a transition to the Grounded state and assign the following condition to it: CustomAnimation, false for a smooth transition between the animations. Uncheck "Has Exit Time" in order to reduce the delay during transition.
3. Double-click the animation state you created and go to its Events section in the Inspector. Add an event at the end of the animation and set its Function to ReturnToIdle.
4. If the animation seems off (e.g. if avatar's feet are not touching the ground or the avatar is swinging around), double-click the animation state you created and check the Bake Into Pose checkboxes under each Root Transform.