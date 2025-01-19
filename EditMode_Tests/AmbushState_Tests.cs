namespace AF.Tests
{
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.AI;

    public class AmbushStateTests
    {
        private GameObject gameObject;
        private AmbushState _state;
        private CharacterManager _mockCharacterManager;
        private StateManager _mockStateManager;

        [SetUp]
        public void SetUp()
        {
            gameObject = new GameObject();
            _state = gameObject.AddComponent<AmbushState>();
            _mockCharacterManager = gameObject.AddComponent<CharacterManager>();
            _state.characterManager = _mockCharacterManager;
            _mockCharacterManager.agent = gameObject.AddComponent<NavMeshAgent>();
            _mockStateManager = gameObject.AddComponent<StateManager>();

        }

        [Test]
        public void OnStateEnter_ShouldResetAgentPathAndSpeed()
        {
            // Act
            _state.OnStateEnter(_mockStateManager);

            Assert.That(_mockCharacterManager.agent.speed, Is.EqualTo(0f));
        }


        [Test]
        public void Tick_ShouldReturnIdleStateWhenShouldAwakeIsTrue()
        {
            // Arrange
            _state.ambushHasFinished = true;

            // Act
            var nextState = _state.Tick(_mockStateManager);

            // Assert
            Assert.That(nextState, Is.SameAs(_state.chaseState));
        }

        [Test]
        public void Tick_ShouldReturnThisWhenShouldAwakeIsFalse()
        {
            // Arrange
            _state.ambushHasFinished = false;

            // Act
            var nextState = _state.Tick(_mockStateManager);

            // Assert
            Assert.That(nextState, Is.SameAs(_state));
        }

    }
}
