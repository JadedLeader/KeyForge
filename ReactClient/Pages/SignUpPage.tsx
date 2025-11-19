
import React, { useState } from "react";
import { Navigate } from "react-router-dom";
import "./SignUpPage.css"

interface SignUpUser { 

    username: string; 
    password: string; 
    email: string;
}
export function SignUpPage() {

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [email, setEmail] = useState(""); 
    const [goToSignIn, SetGoToSignIn] = useState(false);

    if (goToSignIn) { 
        return <Navigate to="/signin" />;
    }


    const handleUsernameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setUsername(e.target.value);
    };

    const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setPassword(e.target.value);
    }

    const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEmail(e.target.value);
    }


    const OnSignUpFormSubmit = async (e: React.FormEvent<HTMLFormElement>) => {

        e.preventDefault();

        try {
            const userSignUpAccount = GenerateAccount();
            await ReachCreateAccountEndpoint(userSignUpAccount);
        } catch (err: any) {
            
        }

        SetGoToSignIn(true);

    };

    function GenerateAccount(): SignUpUser {

        const newUser: SignUpUser = {
            username: username,
            password: password,
            email:  email,
        };

        return newUser;
    }

    async function ReachCreateAccountEndpoint(signingUpUser: SignUpUser) : Promise<any> {

        const res = await fetch("/Account/CreateAccounts", {
            method: "POST",
            headers: {
                "Content-Type": "application/json", 
            },
            body: JSON.stringify(signingUpUser)
        });

        if (!res.ok) { 

            const errorText = await res.text(); 

            throw new Error(errorText);
        }

        const data = await res.json; 

        return data;

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
            <div className="container">

                <div className="header">
                   
                    <img src="/forgeIcon.webp" alt="" className="forge-icon" />
                   
                    <h2>Forge Your Keys</h2>

                </div>

                <form onSubmit={OnSignUpFormSubmit}>
                    <label>Username</label>
                    <input
                        type="text"
                        id="username"
                        name="username"
                        placeholder="Enter username"
                        required
                        value={username}
                        onChange={handleUsernameChange}
                    />

                    <label>Email</label>
                    <input
                        type="email"
                        id="email"
                        name="email"
                        placeholder="you@example.com"
                        required
                        value={email}
                        onChange={handleEmailChange}
                    />

                    <label>Password</label>
                    <input
                        type="password"
                        id="password"
                        name="password"
                        placeholder="Create a strong password"
                        required
                        value={password}
                        onChange={handlePasswordChange}
                    />

                    <button type="submit" className="btn">
                        Sign Up
                    </button>
                </form>

                    <div className="footer">
                        Already registered? <a href="/signin">log in</a>
                    </div>
                </div>
            </div >
        </>
  );
}

export default SignUpPage;