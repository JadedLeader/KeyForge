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

}



export function SignInPage() {

    const [username, setUsername] = useState(""); 
    const [password, setPassword] = useState("");
    const [accountId, setAccountId] = useState("");
    const [shortLivedToken, setShortLivedToken] = useState(""); 
    const [longLivedToken, setLongLivedToken] = useState("");

    const HandleUsernameChanged = (e: React.ChangeEvent<HTMLInputElement>) => { 
        setUsername(e.target.value);
    }

    const HandlePasswordChanged = (e: React.ChangeEvent<HTMLInputElement>) => { 
        setPassword(e.target.value);
    }

    const OnLoginSubmit = async (e: React.FormEvent<SubmitEvent>) => { 

        e.preventDefault();

        const buildUserLogin = BuildUserLogin();


        const loginResponse = await FetchLoginEndpoint(buildUserLogin);

        setAccountId(loginResponse.accountId);

        const buildAuthGenerationRequest = BuildAuth();

        const authResponse = await FetchAuthEndpoint(buildAuthGenerationRequest);

        setShortLivedToken(authResponse.shortLivedToken); 
        setLongLivedToken(authResponse.longLivedToken);

    }

    function BuildUserLogin(): UserLoginRequest { 

        const newUserLogin: UserLoginRequest = { 

            username: username,
            password: password,

        }; 

        return newUserLogin;

    }

    function BuildAuth(): BuildTokenGenerationRequest { 


        const newTokenGeneration: BuildTokenGenerationRequest = {
            accountId: accountId,
        };

        return newTokenGeneration;
    }


    async function FetchLoginEndpoint(loginRequest : UserLogin) : Promise<UserLoginResponse> { 

        const fetchLogin = await fetch("/Auth/Login", {
            method: "GET",
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



    return (


        <p>Hello world!</p>



    );





}

export default SignInPage;