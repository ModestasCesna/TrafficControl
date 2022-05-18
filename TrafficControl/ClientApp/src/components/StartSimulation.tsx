import { useEffect, useState } from "react";
import { Redirect } from "react-router-dom";
import { Gateway } from "../gateway/gateway";

export const StartSimulation = () => {
    const [scenarios, setScenarios] = useState([]);
    const [name, setName] = useState<string>();
    const [scenarioId, setScenarioId] = useState<number>();
    const [redirect, setRedirect] = useState(false);

    useEffect(() => {
        fetchData();
    }, [])

    const fetchData = async () => {
        const data = await Gateway.getScenarios();
        setScenarios(data);
    }

    const nameChange = (e: any) => {
        setName(e.target.value);
    }

    const scenarioChange = (e: any) => {
        setScenarioId(+e.target.value);
    }

    const startSimulation = async () => {
        await Gateway.startSimulation(scenarioId as number, name as string);
        setRedirect(true);
    }

    return <>
        {redirect && <Redirect to="/simulations"/>}
        <h1>Pradėti simuliaciją</h1>
        <div className="card">
            <div className="card-body">
                <form>
                    <div className="row">
                        <div className="col-6">
                            <div className="form-group">
                                <label htmlFor="name">Pavadinimas</label>
                                <input className="form-control" type="text" id="name" onChange={nameChange} />
                            </div>
                        </div>
                        <div className="col-6">
                            <div className="form-group">
                                <label htmlFor="scenario">Scenarijus</label>
                                <select className="form-select" id="scenario" onChange={scenarioChange}>
                                    <option selected>Pasirinkti scenarijų</option>
                                    {scenarios.map((s: any) => <option value={s.id} key={s.id}>{s.name}</option>)}
                                </select>
                            </div>
                        </div>
                    </div>
                </form>
                <div className="row">
                    <div className="col-md-6">
                        <label htmlFor="add"></label>
                        <button className="btn btn-md btn-primary d-block" disabled={!name || !scenarioId} id="add" onClick={startSimulation}>Pradėti</button>
                    </div>
                </div>
            </div>
        </div>
    </>
}