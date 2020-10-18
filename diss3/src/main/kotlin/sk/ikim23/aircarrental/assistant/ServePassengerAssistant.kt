package sk.ikim23.aircarrental.assistant

import OSPABA.*
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.agent.ServiceDeskAgent
import sk.ikim23.aircarrental.entity.Passenger
import sk.ikim23.aircarrental.entity.Place
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.*

class ServePassengerAssistant(mySim: Simulation, val myAgent: ServiceDeskAgent) : ContinualAssistant(ID.ASS_SERVE_PASSENGER, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg ->
            when (msg.code()) {
                M.SERVE_PASSENGER -> {
                    val worker = myAgent.workers.firstOrNull { it.isFree() }
                    if (worker != null) {
                        val passenger = myAgent.qToService.pop()
                        if (passenger != null) {
                            worker.serve(passenger)
                            msg.worker = worker
                            hold(sample(passenger), msg.withCode(M.SERVE_PASSENGER_DONE))
                        }
                    }
                }
                M.SERVE_PASSENGER_DONE -> {
                    val passenger = msg.worker?.free()
                    if (passenger != null) {
                        if (passenger.arrivalPlace == Place.SERVICE_DESK) {
                            notice(MyMessage(mySim(), M.QUEUE_PASSENGER_TO_T3, passenger = passenger))
                        } else {
                            notice(MyMessage(mySim(), M.PASSENGER_LEAVES_SYSTEM, passenger = passenger))
                        }
                    } else {
                        throw IllegalStateException()
                    }
                    hold(.0, MyMessage(mySim(), M.SERVE_PASSENGER))
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }

    private fun sample(passenger: Passenger): Double {
        if (passenger.arrivalPlace == Place.SERVICE_DESK) {
            return Config.serviceTimeOut.sample()
        } // T1 or T2
        return Config.serviceTimeIn.sample()
    }
}