import { useState } from "react";
import { Redirect } from "react-router-dom";
import { Gateway } from "../gateway/gateway";

export const AddScenario = () => {
    const [name, setName] = useState();
    const [networkFile, setNetworkFile] = useState();
    const [routesFile, setRoutesFile] = useState();
    const [redirect, setRedirect] = useState(false);

    const networkFileUpload = (e: any) => {
        setNetworkFile(e.target.files[0]);
    }

    const routesFileUpload = (e: any) => {
        setRoutesFile(e.target.files[0]);
    }

    const nameChange = (e: any) => {
        setName(e.target.value);
    }

    const add = async () => {
        await Gateway.createScenario(name, networkFile, routesFile);
        setRedirect(true);
    }

    return <>
        {redirect && <Redirect to="/scenarios"/>}
        <h1>Pridėti naują scenarijų</h1>
        <div className="card">
            <div className="card-body">
                <form>
                    <div className="row mb-3">
                        <div className="col-6">
                            <div className="form-group">
                                <label htmlFor="name">Pavadinimas</label>
                                <input className="form-control" type="text" id="name" onChange={nameChange} />
                            </div>
                        </div>
                    </div>
                    <div className="row mb-3">
                        <div className="col-6">
                            <div className="form-group">
                                <label htmlFor="networkFile">Tinklo failas</label>
                                <input className="form-control" type="file" id="networkFile" onChange={networkFileUpload} />
                            </div>
                        </div>
                        <div className="col-6">
                            <div className="form-group">
                                <label htmlFor="routeFile">Maršrutų failas</label>
                                <input className="form-control" type="file" id="routeFile" onChange={routesFileUpload} />
                            </div>
                        </div>
                    </div>
                </form>
                <div className="row">
                    <div className="col-md-6">
                        <label htmlFor="add"></label>
                        <button className="btn btn-md btn-primary d-block" id="add" disabled={!name || !networkFile || !routesFile} onClick={() => add()}>Pridėti</button>
                    </div>
                </div>
            </div>
        </div>
    </>;
}