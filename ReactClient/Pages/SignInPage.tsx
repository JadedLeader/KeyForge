import React, { useState } from "react";
import "./SignInPage.css"; 
import { Navigate } from "react-router-dom";
import { error } from "console";
import { json } from "stream/consumers";
import { FetchShortLivedTokenRefresh, FetchAuthEndpoint, FetchLoginEndpoint } from "@/components/api/Auth"

interface UserLoginRequest { 

    username: string; 
    password: string;
}

interface BuildTokenGenerationRequest { 

    accountId: string;

}

interface RefreshShortLivedTokenRequest { 
    accountId: string;
}

export function SignInPage() {

    const [username, setUsername] = useState(""); 
    const [password, setPassword] = useState("");
    const [accountId, setAccountId] = useState("");
    const [shortLivedToken, setShortLivedToken] = useState(""); 
    const [longLivedToken, setLongLivedToken] = useState("");
    const [NavToHomePage, setNavToHomePage] = useState(false); 


   if (NavToHomePage) { 

        console.log("Navigating to home page");

        return <Navigate to="/homepage" />;
    } 


    const HandleUsernameChanged = (e: React.ChangeEvent<HTMLInputElement>) => { 
        setUsername(e.target.value);
    }

    const HandlePasswordChanged = (e: React.ChangeEvent<HTMLInputElement>) => { 
        setPassword(e.target.value);
    }

    function BuildUserLogin(): UserLoginRequest {

        const newUserLogin: UserLoginRequest = {

            username: username,
            password: password,

        };

        return newUserLogin;

    }

    function BuildAuth(accountId: string): BuildTokenGenerationRequest {


        const newTokenGeneration: BuildTokenGenerationRequest = {
            accountId: accountId,
        };

        return newTokenGeneration;
    }

    function BuildShortLivedTokenRequest(accountId: string): RefreshShortLivedTokenRequest {

        const shortLivedtokenRequest: RefreshShortLivedTokenRequest = {
            accountId: accountId,
        };

        return shortLivedtokenRequest;

    }

    const OnLoginSubmit = async (e: React.FormEvent<HTMLFormElement>) => { 

        e.preventDefault();

        const buildUserLogin = BuildUserLogin();

        const loginResponse = await FetchLoginEndpoint(buildUserLogin);

        setAccountId(loginResponse.accountId);

        const buildAuthGenerationRequest = BuildAuth(loginResponse.accountId);

        const authResponse = await FetchAuthEndpoint(buildAuthGenerationRequest);

        setShortLivedToken(authResponse.shortLivedToken);
        setLongLivedToken(authResponse.longLivedToken);

        if (authResponse.accountId != null && authResponse.success == true) { 

            const buildRefreshAuthTokenRequest = BuildShortLivedTokenRequest(loginResponse.accountId);

            const refreshedTokenResponse = await FetchShortLivedTokenRefresh(buildRefreshAuthTokenRequest);

            console.log(refreshedTokenResponse.refreshedToken);

            setShortLivedToken(refreshedTokenResponse.refreshedToken);

            setNavToHomePage(true);

        }

        console.log(username); 
        console.log(password); 
        console.log(accountId); 
        console.log(authResponse.shortLivedToken); 
        console.log(authResponse.longLivedToken);

    }

   



    return (

        <>

            <video
                className="bgvideo"
                autoPlay
                muted
                loop
                playsInline

            >
                <source src="/smoke.mp4" type="video/mp4" />
            </video>

            <div className="center-screen"> 
            <div className="container " >

                <div className="header">

                    <img src="/forgeIcon.webp" alt="" className="forge-icon" />

                    <h2>Login</h2>

                </div>

                <form onSubmit={OnLoginSubmit}>
                    <label>Username</label>
                    <input
                        type="text"
                        id="username"
                        name="username"
                        placeholder="Enter username"
                        required
                        value={username}
                        onChange={HandleUsernameChanged}
                    />

                    
                    <label>Password</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        placeholder="Enter your password"
                        required
                        value={password}
                        onChange={HandlePasswordChanged}
                    />

                    <button type="submit" className="btn">
                        Log in
                    </button>

                    <div className="footer">
                        No Account? <a href="/signup">Sign Up</a>
                    </div>

                </form>
                </div>
            </div>

        </>



    );





}

export default SignInPage;