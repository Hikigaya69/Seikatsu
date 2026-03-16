import HomeNavbar from "../components/home/HomeNavbar";
import HeroBanner from "../components/home/HeroBanner";
import CategoryFilter from "../components/home/CategoryFilter";
import ProductGrid from "../components/home/ProductGrid";
import Footer from "../components/Footer";
export default function Home() {

  return (

    <div className=" min-h-screen">

      <HomeNavbar />
      <div className="p-7" > 
      <HeroBanner />
      </div>

      <div className="max-w-7xl mx-auto px-6">

        <CategoryFilter />

        <ProductGrid />

      </div>
      <Footer />

    </div>

  );
}