import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Gateway } from "../gateway/gateway";

export const Simulations = () => {
    const [simulations, setSimulation] = useState([]);

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        const data = await Gateway.getSimulations();
        setSimulation(data);
    }

    return <>
        <h1 className="mb-4">Simuliacijos</h1>
        <div className="row d-flex">
            <div className="col-4 mb-3">
                <Link to="/start-simulation">
                    <div className="card" style={{minHeight: 140}}>
                        <div className="card-body d-flex align-items-center">
                            <h5 className="card-title">
                                Pradėti simuliaciją
                            </h5>
                        </div>
                    </div>
                </Link>
            </div>

            {simulations.map((s: any) => 
            <div className="col-4 mb-3 cursor-pointer" key={s.id}>
                <Link to={`/simulations/${s.id}`} className="text-decoration-none">
                    <div className="card" style={{minHeight: 140}}>
                        <div className="card-body">
                            <h5 className="card-title">
                                {s.name}
                            </h5>
                            <h6 className="card-subtitle mb-2 text-muted">
                                {new Date(s.created).toLocaleString()}
                            </h6>
                            <div className="row">
                                <div className="col-6 text-muted">Scenarijus</div>
                                <div className="col-6 text-muted">{s.scenarioName}</div>
                            </div>
                            <div className="row">
                                <div className="col-6 text-muted">Statusas</div>
                                <div className="col-6">
                                    {s.state === 1 && <span className="badge bg-info">Vykdoma</span>}
                                    {s.state === 2 && <span className="badge bg-success">Baigta</span>}
                                </div>
                            </div>
                        </div>
                    </div>
                </Link>
            </div>)}
        </div>
    </>;
}