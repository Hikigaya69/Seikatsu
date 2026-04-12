import { useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";

import api from "../Utils/api";

import Navbar from "@/components/Navbar";
import { Input } from "@/components/ui/input";
import { Eye, EyeOff } from "lucide-react";



export default function ResetPassword() {

    const [params] = useSearchParams();
    const navigate = useNavigate();

    const email = params.get("email");
    const token = params.get("token");

    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [error, setError] = useState("");

    // invalid link
    if (!email || !token) {
        return (
            <div className="min-h-screen">
                <Navbar />

                <div className="max-w-md mx-auto mt-20 bg-white shadow rounded-xl p-8 text-center">

                    <p className="text-red-500 font-medium">
                        Invalid or expired reset link.
                    </p>

                    <button
                        onClick={() => navigate("/forgot-password")}
                        className="mt-4 bg-[#284b63] text-white px-6 py-2 rounded-lg"
                    >
                        Request New Link
                    </button>

                </div>
            </div>
        );
    }

    const handleReset = async (e) => {
        e.preventDefault();
       // console.log("email:", email);
       // console.log("token:", token);
        if (password.length < 8) {
            setError("Password must be at least 8 characters.");
            return;
        }

        if (password !== confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        setLoading(true);
        setError("");

        try {

            await api.post("/Auth/reset-password", {
                Email:email,
                Token: token.replace(/ /g, "+"),
                NewPassword: password,
                ConfirmPassword: password

            });

            setSuccess(true);

            // auto redirect after success
            setTimeout(() => {
                navigate("/login");
            }, 2500);

        } catch (err) {

            console.error(err);

            setError(
                err?.response?.data?.message ||
                "Reset link expired or invalid. Please request again."
            );

        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen">
            <Navbar />

            <div className="max-w-md mx-auto mt-20 bg-white shadow rounded-xl p-8">

                <h2 className="text-2xl font-bold text-center text-[#284b63] mb-4">
                    Reset Password
                </h2>

                {success ? (

                    <div className="text-center space-y-4">

                        <p className="text-green-600 font-medium">
                            ✔ Password reset successful!
                        </p>

                        <p className="text-sm text-gray-500">
                            Redirecting to login...
                        </p>

                        <button
                            onClick={() => navigate("/login")}
                            className="bg-[#284b63] text-white px-6 py-2 rounded-lg hover:opacity-90"
                        >
                            Go to Login
                        </button>

                    </div>

                ) : (

                    <form onSubmit={handleReset} className="space-y-4">

                     
                            <div className="relative w-full">
                                <Input
                                    className="bg-background pr-10 h-15"
                                    id="password-toggle"
                                    placeholder="New password"
                                    type={showPassword ? "text" : "password"}
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    required
                                />
                                <button
                                    className="eye-toggle"
                                    onClick={() => setShowPassword(!showPassword)}
                                    type="button"
                                >
                                    {showPassword ? (
                                        <EyeOff className="h-4 w-4" />
                                    ) : (
                                        <Eye className="h-4 w-4" />
                                    )}
                                </button>
                            </div>

                        <p className="text-xs text-gray-400">
                            Must be at least 8 characters
                        </p>

                      
                            <div className="relative w-full">
                                <Input
                                    className="bg-background pr-10 h-15"
                                    id="password-toggle"
                                    placeholder="Confirm Password"
                                    type={showPassword ? "text" : "password"}
                                    value={confirmPassword}
                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                    required
                                />
                                <button
                                    className="eye-toggle"
                                    onClick={() => setShowPassword(!showPassword)}
                                    type="button"
                                >
                                    {showPassword ? (
                                        <EyeOff className="h-4 w-4" />
                                    ) : (
                                        <Eye className="h-4 w-4" />
                                    )}
                                </button>
                            </div>

                        {error && (
                            <p className="text-sm text-red-500">{error}</p>
                        )}

                        <button
                            type="submit"
                            disabled={loading}
                            className="w-full bg-[#284b63] text-white py-2 rounded-lg hover:opacity-90 disabled:opacity-50"
                        >
                            {loading ? "Resetting..." : "Reset Password"}
                        </button>

                    </form>

                )}

            </div>
        </div>


    );
}