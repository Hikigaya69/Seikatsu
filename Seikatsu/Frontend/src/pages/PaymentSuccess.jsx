import {
  CheckCircle,
  ShoppingBag
} from "lucide-react";

import { useNavigate } from "react-router-dom";

import StoreLayout from "../layouts/StoreLayout";

export default function PaymentSuccess() {

  const navigate = useNavigate();

  return (

    <StoreLayout>

      <div className="min-h-[80vh] flex items-center justify-center px-6">

        <div className="bg-white rounded-3xl shadow-xl p-14 max-w-xl w-full text-center border">

          <div className="flex justify-center mb-6">

            <div className="w-24 h-24 rounded-full bg-green-100 flex items-center justify-center">

              <CheckCircle
                size={54}
                className="text-green-600"
              />

            </div>

          </div>


          <h1 className="text-4xl font-bold text-gray-800 mb-4">

            Payment Successful

          </h1>


          <p className="text-gray-500 text-lg leading-relaxed mb-8">

            Your order has been placed successfully.
            Thank you for shopping with Seikatsu.

          </p>


          <div className="bg-[#f8fafc] rounded-2xl p-5 mb-8">

            <div className="flex items-center justify-center gap-3 text-gray-700">

              <ShoppingBag size={20}/>

              <span>
                Your items will be processed soon
              </span>

            </div>

          </div>


          <div className="flex gap-4">

            <button
              onClick={()=>
                navigate("/home")
              }
              className="flex-1 border border-gray-300 py-3 rounded-xl hover:bg-gray-100 transition"
            >

              Continue Shopping

            </button>


            <button
              onClick={()=>
                navigate("/profile")
              }
              className="flex-1 bg-[#284b63] text-white py-3 rounded-xl hover:opacity-90 transition"
            >

              View Orders

            </button>

          </div>

        </div>

      </div>

    </StoreLayout>

  );

}