import HomeNavbar from "../components/home/HomeNavbar";
import Footer from "../components/Footer";

export default function StoreLayout({ children }) {

  return (

    <div className="min-h-screen flex flex-col">

      {/* NAVBAR */}
      <HomeNavbar />

      <main className="flex-grow">
        {children}
      </main>

      {/* FOOTER */}
      <Footer />

    </div>

  );

}