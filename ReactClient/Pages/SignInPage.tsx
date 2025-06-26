import React, { useState } from "react";
import "./SignInPage.css"; 
import { Navigate } from "react-router-dom";
import { error } from "console";
import { json } from "stream/consumers";

interface UserLoginRequest { 

    username: string; 
    password: string;
}

interface UserLoginResponse { 
    accountId: string;
}

interface BuildTokenGenerationRequest { 

    accountId: string;

}

interface BuildTokenGenerationResponse { 
    accountId: string;
    shortLivedToken: string; 
    longLivedToken: string;
    successful: bool;
    details: string;

}

interface RefreshShortLivedTokenRequest { 
    accountId: string;
}

interface RefreshShortLivedTokenResponse { 

    accountId: string; 
    successful: bool; 
    refreshedToken: string;
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

    const OnLoginSubmit = async (e: React.FormEvent<HTMLFormElement>) => { 

        e.preventDefault();

        const buildUserLogin = BuildUserLogin();

        const loginResponse = await FetchLoginEndpoint(buildUserLogin);

        setAccountId(loginResponse.accountId);

        const buildAuthGenerationRequest = BuildAuth(loginResponse.accountId);

        const authResponse = await FetchAuthEndpoint(buildAuthGenerationRequest);

        setShortLivedToken(authResponse.shortLivedToken);
        setLongLivedToken(authResponse.longLivedToken);

        if (authResponse.accountId != null && authResponse.successful == true) { 

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


    async function FetchLoginEndpoint(loginRequest : UserLoginRequest) : Promise<UserLoginResponse> { 

        const fetchLogin = await fetch("/Auth/UserLogin", {
            method: "POST",
            headers: {
                'Content-type': "application/json"
            },
            body: JSON.stringify(loginRequest),
        });

        if (!fetchLogin.ok) { 

            const errorText = await fetchLogin.text(); 

            throw new Error(errorText);
        }

        const responseData = await fetchLogin.json() as UserLoginResponse;

        console.log("retrieved account login response"); 
        console.log("Account ID", responseData.accountId);

        return responseData;


    }

    async function FetchAuthEndpoint(buildTokenGenerationRequest: BuildTokenGenerationRequest) : Promise<BuildTokenGenerationResponse> { 

        const fetchAuth = await fetch("/Auth/CreateInitialKey", {
            method: "POST",
            headers: {
                'Content-type': "application/json"
            },
            body: JSON.stringify(buildTokenGenerationRequest),
        });

        if (!fetchAuth.ok) { 

            const authErrorText = await fetchAuth.text(); 

            throw new Error(authErrorText);

        }

        const authResponseData = await fetchAuth.json() as BuildTokenGenerationResponse; 


        return authResponseData;

    }

    function BuildShortLivedTokenRequest(accountId: string): RefreshShortLivedTokenRequest { 

        const shortLivedtokenRequest: RefreshShortLivedTokenRequest = {
            accountId: accountId,
        }; 

        return shortLivedtokenRequest;

    }

    async function FetchShortLivedTokenRefresh(shortLivedTokenRequest: RefreshShortLivedTokenRequest): Promise<RefreshShortLivedTokenResponse>{ 


        const fetchShortLivedTokenEndpoint = await fetch("/Auth/UpdateShortLivedKey", {
            method: "PUT",
            headers: {
                'Content-type': "application/json"
            },
            body: JSON.stringify(shortLivedTokenRequest),
        }); 

        if (!fetchShortLivedTokenEndpoint.ok) { 

            const errorText = await fetchShortLivedTokenEndpoint.text(); 

            throw new Error(errorText);
            
        }

        const responseData = await fetchShortLivedTokenEndpoint.json() as RefreshShortLivedTokenResponse;

        return responseData;

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

            <div className="container">

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

        </>



    );





}

export default SignInPage;