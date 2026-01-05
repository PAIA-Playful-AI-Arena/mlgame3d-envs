using System;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace Unity.MLAgents.Policies
{
    /// <summary>
    /// The HybridPolicy combines two policies: one for decision making and one for communication.
    /// This is useful for scenarios where we want to use HeuristicPolicy for decision making
    /// but still need to communicate with the Python side.
    /// </summary>
    internal class HybridPolicy : IPolicy
    {
        private IPolicy m_DecisionPolicy;
        private IPolicy m_CommunicationPolicy;

        /// <summary>
        /// Creates a new HybridPolicy.
        /// </summary>
        /// <param name="decisionPolicy">The policy used for making decisions (typically HeuristicPolicy).</param>
        /// <param name="communicationPolicy">The policy used for communication (typically RemotePolicy).</param>
        public HybridPolicy(IPolicy decisionPolicy, IPolicy communicationPolicy)
        {
            m_DecisionPolicy = decisionPolicy;
            m_CommunicationPolicy = communicationPolicy;
        }

        /// <inheritdoc />
        public void RequestDecision(AgentInfo info, List<ISensor> sensors)
        {
            // Use the decision policy to request a decision
            m_DecisionPolicy.RequestDecision(info, sensors);
            
            // Also use the communication policy to send observations to Python
            m_CommunicationPolicy.RequestDecision(info, sensors);
        }

        /// <inheritdoc />
        public ref readonly ActionBuffers DecideAction()
        {
            // Call the communication policy's DecideAction to ensure proper communication flow
            // but ignore its result
            m_CommunicationPolicy.DecideAction();
            
            // Use the decision policy to decide the actual action
            return ref m_DecisionPolicy.DecideAction();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            m_DecisionPolicy.Dispose();
            m_CommunicationPolicy.Dispose();
        }
    }
}