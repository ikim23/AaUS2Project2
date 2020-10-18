package sk.ikim23.aircarrental.assistant

import OSPABA.*
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.agent.OutsideAgent
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.entity.Passenger
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.random.NumberRNG
import sk.ikim23.aircarrental.random.Rand
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

class PassengerArrivalSharedAssistant(mySim: Simulation, val myAgent: OutsideAgent, id: Int, val place: Place, val rand: Rand, val passengerSize: NumberRNG) : ContinualAssistant(id, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg ->
            when (msg.code()) {
                M.INIT -> {
                    hold(rand.sample(mySim().currentTime()), msg.withCode(M.PASSENGER_ARRIVED))
                }
                M.PASSENGER_ARRIVED -> {
                    if (myAgent.canProduceNextArrival()) {
                        hold(rand.sample(mySim().currentTime()), msg)
                    }
                    val mArrival = MyMessage(mySim(), M.PASSENGER_ARRIVED, passenger = Passenger(mySim().currentTime(), place, passengerSize.sample()))
                    notice(mArrival)
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}