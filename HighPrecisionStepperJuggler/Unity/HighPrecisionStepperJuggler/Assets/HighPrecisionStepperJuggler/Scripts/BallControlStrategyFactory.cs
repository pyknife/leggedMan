﻿using System.Collections.Generic;
using UnityEngine;
using c = HighPrecisionStepperJuggler.Constants;

namespace HighPrecisionStepperJuggler
{
    public static class BallControlStrategyFactory
    {
        public static IBallControlStrategy GetBouncing()
        {
            return new BallControlStrategy((ballData, machineController, instructionCount) =>
            {
                var moveTime = 0.1f;
                machineController.SendInstructions(new List<HLInstruction>()
                {
                    new HLInstruction(0.02f, 0f, 0f, moveTime),
                    new HLInstruction(0.05f, 0f, 0f, moveTime),
                    new HLInstruction(0.02f, 0f, 0f, moveTime),
                    new HLInstruction(0.05f, 0f, 0f, moveTime),
                    new HLInstruction(0.02f, 0f, 0f, moveTime),
                    new HLInstruction(0.05f, 0f, 0f, moveTime),
                    new HLInstruction(0.02f, 0f, 0f, moveTime),
                    new HLInstruction(0.05f, 0f, 0f, moveTime),
                });
                ballData.ResetVelocityAccumulation();
                return true;
            }, 1);
        }

        public static IBallControlStrategy ContinuousBouncing(int duration)
        {
            return new BallControlStrategy((ballData, machineController, instructionCount) =>
            {
                if (ballData.CurrentPositionVector.z < 150f)
                {
                    // distance away from plate:
                    var p_x = -ballData.CurrentPositionVector.x * c.k_p;
                    var p_y = ballData.CurrentPositionVector.y * c.k_p;

                    // mean velocity of ball:
                    var velocityVector = ballData.GetVelocityVector();
                    var d_x = -velocityVector.x * c.k_d;
                    var d_y = velocityVector.y * c.k_d;

                    var xCorrection = Mathf.Clamp(p_x + d_x, c.MinTiltAngle, c.MaxTiltAngle);
                    var yCorrection = Mathf.Clamp(p_y + d_y, c.MinTiltAngle, c.MaxTiltAngle);

                    var moveTime = 0.1f;
                    machineController.SendInstructions(new List<HLInstruction>()
                    {
                        new HLInstruction(0.06f, xCorrection, yCorrection, moveTime),
                        new HLInstruction(0.05f, 0f, 0f, moveTime),
                    });

                    ballData.ResetVelocityAccumulation();
                    return true;
                }

                return false;
            }, duration);
        }

        public static IBallControlStrategy BalancingAtHeight(float height, int duration)
        {
            return new BallControlStrategy((ballData, machineController, instructionCount) =>
            {
                if (ballData.CurrentPositionVector.z < float.MaxValue)
                {
                    // distance away from plate:
                    var p_x = -ballData.CurrentPositionVector.x * c.k_p;
                    var p_y = ballData.CurrentPositionVector.y * c.k_p;

                    // mean velocity of ball:
                    var velocityVector = ballData.GetVelocityVector();
                    var d_x = -velocityVector.x * c.k_d * 0.5f;
                    var d_y = velocityVector.y * c.k_d * 0.5f;

                    var xCorrection = Mathf.Clamp(p_x + d_x, c.MinTiltAngle, c.MaxTiltAngle);
                    var yCorrection = Mathf.Clamp(p_y + d_y, c.MinTiltAngle, c.MaxTiltAngle);

                    var moveTime = 0.1f;
                    machineController.SendInstructions(new List<HLInstruction>()
                    {
                        new HLInstruction(height, xCorrection, yCorrection, moveTime),
                    });

                    ballData.ResetVelocityAccumulation();
                    return true;
                }

                return false;
            }, duration);
        }

        public static IBallControlStrategy ContinuousBouncingStrong(int duration)
        {
            return new BallControlStrategy((ballData, machineController, instructionCount) =>
            {
                if (ballData.CurrentPositionVector.z < 190f)
                {
                    // distance away from plate:
                    var p_x = -ballData.CurrentPositionVector.x * c.k_p;
                    var p_y = ballData.CurrentPositionVector.y * c.k_p;

                    // mean velocity of ball:
                    var velocityVector = ballData.GetVelocityVector();
                    var d_x = -velocityVector.x * c.k_d;
                    var d_y = velocityVector.y * c.k_d;

                    var xCorrection = Mathf.Clamp(p_x + d_x, c.MinTiltAngle / 2f, c.MaxTiltAngle / 2f);
                    var yCorrection = Mathf.Clamp(p_y + d_y, c.MinTiltAngle / 2f, c.MaxTiltAngle / 2f);

                    var moveTime = 0.1f;
                    machineController.SendInstructions(new List<HLInstruction>()
                    {
                        new HLInstruction(0.08f, xCorrection, yCorrection, moveTime),
                        new HLInstruction(0.05f, 0f, 0f, moveTime),
                    });

                    ballData.ResetVelocityAccumulation();
                    return true;
                }

                return false;
            }, duration);
        }

        public static IBallControlStrategy GoTo(float height, float time = 0.5f)
        {
            return new BallControlStrategy((ballData, machineController, instructionCount) =>
            {
                machineController.SendInstructions(new List<HLInstruction>()
                {
                    new HLInstruction(height, 0f, 0f, time),
                });
                ballData.ResetVelocityAccumulation();
                return true;
            }, 1);
        }

        public static IBallControlStrategy HighPlateBalancingAt(Vector2 position, int duration)
        {
            return new BallControlStrategy((ballData, machineController, instructionCount) =>
            {
                if (ballData.CurrentPositionVector.z < float.MaxValue)
                {
                    // distance away from plate:
                    var p_x = (position.x - ballData.CurrentPositionVector.x) * c.k_p;
                    var p_y = (position.y + ballData.CurrentPositionVector.y) * c.k_p;

                    // mean velocity of ball:
                    var velocityVector = ballData.GetVelocityVector();
                    var d_x = -velocityVector.x * c.k_d;
                    var d_y = velocityVector.y * c.k_d;

                    var xCorrection = Mathf.Clamp(p_x + d_x, c.MinTiltAngle, c.MaxTiltAngle);
                    var yCorrection = Mathf.Clamp(p_y + d_y, c.MinTiltAngle, c.MaxTiltAngle);

                    var moveTime = 0.1f;
                    machineController.SendInstructions(new List<HLInstruction>()
                    {
                        new HLInstruction(0.08f, xCorrection, yCorrection, moveTime),
                    });

                    ballData.ResetVelocityAccumulation();
                    return true;
                }

                return false;
            }, duration);
        }

        public static IBallControlStrategy HighPlateCircleBalancing(float radius, int duration)
        {
            return new BallControlStrategy((ballData, machineController, instructionCount) =>
            {
                if (ballData.CurrentPositionVector.z < float.MaxValue)
                {
                    var position = new Vector2(
                        Mathf.Cos(Time.realtimeSinceStartup * 2f * Mathf.PI * 0.5f) * radius,
                        Mathf.Sin(Time.realtimeSinceStartup * 2f * Mathf.PI * 0.5f) * radius);
                    // distance away from plate:
                    var p_x = (position.x - ballData.CurrentPositionVector.x) * c.k_p;
                    var p_y = (position.y + ballData.CurrentPositionVector.y) * c.k_p;

                    // mean velocity of ball:
                    var velocityVector = ballData.GetVelocityVector();
                    var d_x = -velocityVector.x * c.k_d;
                    var d_y = velocityVector.y * c.k_d;

                    var xCorrection = Mathf.Clamp(p_x + d_x, c.MinTiltAngle, c.MaxTiltAngle);
                    var yCorrection = Mathf.Clamp(p_y + d_y, c.MinTiltAngle, c.MaxTiltAngle);

                    var moveTime = 0.1f;
                    machineController.SendInstructions(new List<HLInstruction>()
                    {
                        new HLInstruction(0.08f, xCorrection, yCorrection, moveTime),
                    });

                    ballData.ResetVelocityAccumulation();
                    return true;
                }

                return false;
            }, duration);
        }
    }
}
