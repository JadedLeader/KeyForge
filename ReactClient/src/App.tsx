import { useEffect, useState, type ChangeEvent, type FormEvent } from 'react';
import './App.css';

interface Forecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}

function App() {

    const [forecasts, setForecasts] = useState<Forecast[]>();
    const [str, setStr] = useState<string | null>(null);
    const [username, setUsernameVar] = useState<string>("");
    const [password, setPasswordVar] = useState<string>("");


    useEffect(() => {
        populateWeatherData();
    }, []);

    useEffect(() => {
        (async () => {
            const result = await populateString("hello, react");
            setStr(result);
        })();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                {forecasts.map(forecast =>
                    <tr key={forecast.date}>
                        <td>{forecast.date}</td>
                        <td>{forecast.temperatureC}</td>
                        <td>{forecast.temperatureF}</td>
                        <td>{forecast.summary}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    const formContents =
        <form onSubmit={HandleSubmission}>
            <div>
                <textarea value={username} onChange={setUsername} placeholder="Insert username"> </textarea>
            </div>
            <div>
                <textarea value={password} onChange={setPassword} placeholder="Insert password"> </textarea>
            </div>

            <button type="submit">
                Submit
            </button>
        </form>

    return (
        <div>

            <h1>My async string:</h1>
            {
                str == null ? <p>Loading string</p> : <p>{str}</p>
            }

            {formContents}

            <h1 id="tableLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>

            {contents}

        </div>
    );


    async function populateString(a: string): Promise<string> {

        return a + "!!";
    }

    async function populateWeatherData() {
        const response = await fetch('/weatherforecast');
        const data = await response.json();
        setForecasts(data);
    }

    function setUsername(username: ChangeEvent<HTMLTextAreaElement>) {
        setUsernameVar(username.target.value);
    }

    function setPassword(password: ChangeEvent<HTMLTextAreaElement>) {
        setPasswordVar(password.target.value);
    }


    function HandleSubmission(e: FormEvent<HTMLFormElement>) {
        e.preventDefault();
        const loginData = { username, password };
        console.log(loginData);
    }
}

export default App;