package sk.ikim23.aircarrental.assistant

import OSPABA.*
import sk.ikim23.aircarrental.M
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.Config
import sk.ikim23.aircarrental.sim.withCast

class TakeOffBusSharedAssistant(mySim: Simulation, myAgent: CommonAgent, id: Int) : ContinualAssistant(id, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg ->
            when (msg.code()) {
                M.TAKE_OFF_FROM_BUS -> {
                    val passenger = msg.bus?.pop()
                    if (passenger != null) {
                        val mTakeOffDone = MyMessage(mySim(), M.TAKE_OFF_FROM_BUS_DONE, bus = msg.bus, passenger = passenger)
                        val takeOffTime = (1..passenger.size).sumByDouble { Config.takeOffTime.sample() }
                        hold(takeOffTime, mTakeOffDone)
                    } else {
                        notice(MyMessage(mySim(), M.BUS_EMPTIED, bus = msg.bus))
                    }
                }
                M.TAKE_OFF_FROM_BUS_DONE -> {
                    notice(MyMessage(mySim(), M.PROCESS_PASSENGER_FROM_BUS, passenger = msg.passenger))
                    val mTakeOff = MyMessage(mySim(), M.TAKE_OFF_FROM_BUS, bus = msg.bus)
                    hold(.0, mTakeOff)
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}