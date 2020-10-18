package sk.ikim23.aircarrental.assistant

import OSPABA.*
import sk.ikim23.aircarrental.*
import sk.ikim23.aircarrental.entity.Bus
import sk.ikim23.aircarrental.entity.Passenger
import sk.ikim23.aircarrental.message.MyMessage
import sk.ikim23.aircarrental.sim.Config
import sk.ikim23.aircarrental.sim.MessageNotImplementedException
import sk.ikim23.aircarrental.sim.withCast

interface ILoadOnBusAssistantMaster {
    fun nextPassenger(bus: Bus?): Passenger?
}

class LoadOnBusSharedAssistant(mySim: Simulation, myAgent: CommonAgent, id: Int, val master: ILoadOnBusAssistantMaster) : ContinualAssistant(id, mySim, myAgent) {
    override fun processMessage(messageForm: MessageForm) {
        withCast(this, messageForm) { msg ->
            when (msg.code()) {
                M.LOAD_PASSENGER -> {
                    val passenger = master.nextPassenger(msg.bus)
                    if (passenger != null) {
                        val mLoadDone = MyMessage(mySim(), M.LOAD_PASSENGER_DONE, bus = msg.bus, passenger = passenger)
                        val loadTime = (1..passenger.size).sumByDouble { Config.loadTime.sample() }
                        hold(loadTime, mLoadDone)
                    } else {
                        notice(MyMessage(mySim(), M.BUS_LOADED, bus = msg.bus))
                    }
                }
                M.LOAD_PASSENGER_DONE -> {
                    msg.bus?.add(msg.passenger!!)
                    val mLoad = MyMessage(mySim(), M.LOAD_PASSENGER, bus = msg.bus)
                    hold(.0, mLoad)
                }
                else -> throw MessageNotImplementedException(msg, this)
            }
        }
    }
}